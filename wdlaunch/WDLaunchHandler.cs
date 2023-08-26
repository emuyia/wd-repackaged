using Amemiya.Extensions;
using LECommonLibrary;
using LEProc;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LEProc.LoaderWrapper;

namespace WDLaunch
{
	public static class WDLaunchHandler
	{
		// Adapted from [Locale Emulator], licensed under LGPL 3.0
		// Original source: [https://github.com/xupefei/Locale-Emulator/blob/master/LEProc/LoaderWrapper.cs]
		public static void Start(Form launcherForm, string file, bool localeEmulation, bool autoLaunching, string memoryInjectionString)
		{
			try
			{
				string absPath = SystemHelper.EnsureAbsolutePath(file);

				IntPtr locLEB = IntPtr.Zero;

				if (localeEmulation)
				{
					CultureInfo KRCultureInfo = new CultureInfo("ko");

					var registries = RegistryEntriesLoader.GetRegistryEntries(false);

					_leb = new LEB
					{
						AnsiCodePage = (uint)KRCultureInfo.TextInfo.ANSICodePage,
						OemCodePage = (uint)KRCultureInfo.TextInfo.OEMCodePage,
						LocaleID = (uint)KRCultureInfo.TextInfo.LCID,
						DefaultCharset = 129,
						HookUILanguageAPI = 0,
						DefaultFaceName = new byte[64]
					};

					SetTimezone("Korea Standard Time");

					var newLEB = ArrayExtensions.StructToBytes(_leb);
					newLEB = newLEB.CombineWith(_registry.GetBinaryData());

					locLEB = Marshal.AllocHGlobal(newLEB.Length);
					Marshal.Copy(newLEB, 0, locLEB, newLEB.Length);

					registries?.ToList()
							.ForEach(
								item =>
									AddRegistryRedirectEntry(item.Root,
										item.Key,
										item.Name,
										item.Type,
										item.GetValue(KRCultureInfo)));

					AttachConsole(ATTACH_PARENT_PROCESS);
				}

				Task WhiteDayGame = Task.Run(() =>
				{
					STARTUPINFO startInfo = new STARTUPINFO();

					if (!autoLaunching) memoryInjectionString = "launcher";

					var ret = LeCreateProcess(locLEB, // leb
									   absPath, // application name
									   $"\"{absPath}\"", // command line
									   Path.GetDirectoryName(absPath), // current directory
									   CREATE_SUSPENDED, // creation flags
									   ref startInfo, // startup info
									   out PROCESS_INFORMATION procInfo, // process information
									   IntPtr.Zero, // process attributes
									   IntPtr.Zero, // thread attributes
									   IntPtr.Zero, // environment
									   IntPtr.Zero); // token


					byte[] buffer = Encoding.ASCII.GetBytes($"{memoryInjectionString}\0");

					// Memory address in whiteday.exe for string to redirect launch operation
					IntPtr targetAddress = (IntPtr)0x40B128;

					// While suspended, modify memory
					bool result = WriteProcessMemory(procInfo.hProcess, targetAddress, buffer, (uint)buffer.Length, out _);

					// Resume thread
					ResumeThread(procInfo.hThread);

					if (autoLaunching && memoryInjectionString == "whiteday") ClickWhiteDayWindowHandle();

					if (localeEmulation) Marshal.FreeHGlobal(locLEB);

					launcherForm.Invoke(new Action(() => launcherForm.Hide()));

					if (ret == 0)
					{
						WaitForSingleObject(procInfo.hProcess, INFINITE);
						CloseHandle(procInfo.hProcess);
						CloseHandle(procInfo.hThread);

						launcherForm.Invoke(new Action(() => launcherForm.Show()));

						Application.Restart();
						Environment.Exit(0);
					}
				});

				WhiteDayGame.ContinueWith(t =>
				{
					// t.Exception is an AggregateException containing the original exception(s)
					MessageBox.Show($"WhiteDayGame Task\n\n {t.Exception.InnerException}");
				}, TaskContinuationOptions.OnlyOnFaulted);

			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private static void ClickWhiteDayWindowHandle()
		{
			const int MAX_ATTEMPTS = 50;
			int attempts = 0;

			bool isKR = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Sonnori\WhiteDay" + @"\Option", "lang", "0").ToString() == "1";

			string buttonString = isKR ? "&Yes" : "Launch Game";

			if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt) buttonString = isKR ? "&No" : "C";

			while (attempts < MAX_ATTEMPTS)
			{
				IntPtr hwndParent = WaitForWindow("WhiteDay", TimeSpan.FromSeconds(10));
				if (hwndParent == IntPtr.Zero)
				{
					Console.WriteLine("Couldn't find the WhiteDay window on attempt " + (attempts + 1));
					attempts++;
					Thread.Sleep(100); // Wait for a short duration before searching again
					continue; // Go to next iteration, which will attempt to find the window again
				}

				Console.WriteLine("WhiteDay window found.");

				IntPtr hwndButton = IntPtr.Zero;
				bool buttonFound = false;

				EnumChildWindows(hwndParent, new EnumWindowsProc((hWnd, lParam) =>
				{
					StringBuilder sb = new StringBuilder(1024);
					GetWindowText(hWnd, sb, sb.Capacity);

					if (sb.ToString() == buttonString)
					{
						hwndButton = hWnd;
						Console.WriteLine($"{buttonString} button found.");
						buttonFound = true; // Stop enumeration
						return false;
					}
					return true; // Continue enumeration
				}), IntPtr.Zero);

				if (buttonFound)
				{
					SendMessage(hwndButton, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
					Console.WriteLine("Sent click");
					return; // Successful completion
				}

				// If button is not found, increment attempts and forget the window for next iteration
				attempts++; // Increment the count of attempts
				hwndParent = IntPtr.Zero;
				Thread.Sleep(100);
			}
			MessageBox.Show($"Couldn't find the {buttonString} button after multiple attempts.");
		}

		private static IntPtr WaitForWindow(string title, TimeSpan timeout)
		{
			IntPtr hwnd = IntPtr.Zero;
			DateTime start = DateTime.Now;

			while (DateTime.Now - start < timeout)
			{
				hwnd = FindWindow(null, title);
				if (hwnd != IntPtr.Zero)
				{
					break;
				}
				// Wait a bit before trying again
				Thread.Sleep(100);
			}

			return hwnd;
		}

		// Adapted from [Locale Emulator], licensed under LGPL 3.0
		// Original source: [https://github.com/xupefei/Locale-Emulator/blob/master/LEProc/LoaderWrapper.cs]
		private static void SetTimezone(string value)
		{
			if (value.Length > 32)
				throw new Exception("String too long.");

			if (false == Enumerable.Any(TimeZoneInfo.GetSystemTimeZones(), item => item.Id == value))
				throw new Exception($"Timezone \"{value}\" not found in your system.");

			var tzi = TimeZoneInfo.FindSystemTimeZoneById(value);
			_leb.Timezone.SetStandardName(tzi.StandardName);
			_leb.Timezone.SetDaylightName(tzi.StandardName);

			var tzi2 = ReadTZIFromRegistry(value);
			_leb.Timezone.Bias = tzi2.Bias;
			_leb.Timezone.StandardBias = tzi2.StandardBias;
			_leb.Timezone.DaylightBias = 0; //tzi2.DaylightBias;

			return;
		}
		private struct _REG_TZI_FORMAT
		{
			internal int Bias;
			internal int StandardBias;
			internal int DaylightBias;
			internal _SYSTEMTIME StandardDate;
			internal _SYSTEMTIME DaylightDate;
		}
		private static _REG_TZI_FORMAT ReadTZIFromRegistry(string id)
		{
			var tzi =
				(byte[])
				Registry.GetValue(
								  $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\{id}",
								  "TZI",
								  null);

			return BytesToStruct<_REG_TZI_FORMAT>(tzi);
		}
		private static bool AddRegistryRedirectEntry(
			string root,
			string subkey,
			string valueName,
			string dataType,
			string data)
		{
			return _registry.AddRegistryEntry(root,
											  subkey,
											  valueName,
											  dataType,
											  data);
		}


		// ==============================

		// Adapted from [Locale Emulator], licensed under LGPL 3.0
		// Original source: [https://github.com/xupefei/Locale-Emulator/blob/master/LEProc/LoaderWrapper.cs]
		[DllImport("LoaderDll.dll", CharSet = CharSet.Unicode)]
		public static extern uint LeCreateProcess(IntPtr leb,
												  [MarshalAs(UnmanagedType.LPWStr), In] string applicationName,
												  [MarshalAs(UnmanagedType.LPWStr), In] string commandLine,
												  [MarshalAs(UnmanagedType.LPWStr), In] string currentDirectory,
												  uint creationFlags,
												  ref STARTUPINFO startupInfo,
												  out PROCESS_INFORMATION processInformation,
												  IntPtr processAttributes,
												  IntPtr threadAttributes,
												  IntPtr environment,
												  IntPtr token);

		private static LEB _leb;
		private static readonly LERegistryRedirector _registry = new LERegistryRedirector(0);

		//private const uint CREATE_NORMAL = 0x00000000;
		private const uint CREATE_SUSPENDED = 0x00000004;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AttachConsole(uint dwProcessId);
		const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool CloseHandle(IntPtr hHandle);

		const uint INFINITE = 0xFFFFFFFF;

		// Memory injection
		[DllImport("kernel32.dll")]
		public static extern uint ResumeThread(IntPtr hThread);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct STARTUPINFO
		{
			public Int32 cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);


		// Window handles

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		const int BM_CLICK = 0x00F5;
	}
}
