using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using WDHelper.Properties;

namespace WDHelper
{
	class WDHelper
	{
		static void Main(string[] args)
		{
			string arg1 = args.Length > 1 ? args[1] : "";
			string arg2 = args.Length > 2 ? args[2] : "";
			string arg3 = args.Length > 3 ? args[3] : "";

			AdminTasks adminTasks = new AdminTasks();

			if (args.Length > 0)
			{
				adminTasks.SetPath(args[0]);
			}

			try
			{
				switch (arg1)
				{
					case "LANG_KR":
						adminTasks.PATCHFILES(true);
						break;
					case "LANG_EN":
						adminTasks.PATCHFILES(false);
						break;
					case "WDLAUNCH":
						adminTasks.WDLAUNCH();
						break;
					case "WRAPD3D":
						adminTasks.WRAPD3D(arg2, arg3);
						break;
					case "D3DCRO":
						adminTasks.D3DCRO();
						break;
					case "D3DDGV":
						adminTasks.D3DDGV();
						break;
					case "DGV_AA":
						adminTasks.DGV_AA(arg2);
						break;
					case "DGV_TEXFILT":
						adminTasks.DGV_TEXFILT(arg2);
						break;
					case "DGV_VSYNC":
						adminTasks.DGV_VSYNC(arg2);
						break;
					case "DGV_FAKE":
						adminTasks.DGV_FAKE(arg2);
						break;
					case "DGV_SAR":
						adminTasks.DGV_SAR(arg2);
						break;
					case "DGV_CAPMOUSE":
						adminTasks.DGV_CAPMOUSE(arg2);
						break;
					case "DGV_SCREENTOGGLE":
						adminTasks.DGV_SCREENTOGGLE(arg2);
						break;
					case "DGV_DEFAULTWINDOWED":
						adminTasks.DGV_DEFAULTWINDOWED(arg2);
						break;
					case "UPDATE":
						adminTasks.UPDATE(arg2, arg3);
						break;
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Error: {e.Message}");
				Console.WriteLine();
				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();
			}
		}
	}
	public class AdminTasks
	{
		string path = "";
		private readonly string[] files = { "Launcher.dll", "whiteday.exe", "WhiteDay.dll", "WhiteDay_p4.dll", "mod_beanbag.dll" };

		readonly Utils utils = new Utils();

		public void SetPath(string pathArg)
		{
			path = pathArg;
		}

		public void WDLAUNCH()
		{
			Process.Start(Path.Combine(path, "wdlaunch.exe"));
		}

		public void WRAPD3D(string setting, string wrapper)
		{
			Console.WriteLine($"Changing wrapper setting to \"{setting}\" (\"{wrapper}\")\n");

			bool settingBool = Convert.ToBoolean(setting);
			string D3D8 = Path.Combine(path, "d3d8.dll");
			string wrapperBackup = Path.Combine(path, $"d3d8.{wrapper.ToLower()}");
			string wrapperAlternative = Path.Combine(path, $"d3d8.{(wrapper == "DGV" ? "cro" : "dgv")}");
			string DGV_CONF = Path.Combine(path, "dgVoodoo.conf");

			utils.ModifyDGVConfig(DGV_CONF, "DirectX", "DisableAndPassThru", settingBool ? "false" : "true");

			if (settingBool)
			{
				if (File.Exists(wrapperBackup))
				{
					File.Delete(D3D8);
					File.Move(wrapperBackup, D3D8);
					Console.WriteLine($"Success.");
				}
				else
				{
					if (File.Exists(wrapperAlternative))
					{
						File.Delete(D3D8);
						File.Move(wrapperAlternative, D3D8);
						Console.WriteLine($"Warning: Could not find \"{Path.GetFileName(wrapperBackup)}\", " +
							              $"however {Path.GetFileName(wrapperAlternative)} was found, and has been used instead.");
						utils.CloseInSeconds(4);
					}
					else
					{
						Console.WriteLine($"\nFailed: Both \"{Path.GetFileName(wrapperBackup)}\" and " +
										  $"\"{Path.GetFileName(wrapperAlternative)}\" are missing.");
						utils.CloseInSeconds(4);

					}
				}
			}
			else
			{
				if (File.Exists(D3D8))
				{
					File.Delete(wrapperBackup);
					File.Move(D3D8, wrapperBackup);
					Console.WriteLine($"Success.");
				}
			}
		}

		public void SwitchD3D(string desiredAuthor, string currentAuthor, string desiredDll, string currentDll)
		{
			string D3D8 = Path.Combine(path, "d3d8.dll");
			string desiredDllBackup = Path.Combine(path, desiredDll);
			string currentDllBackup = Path.Combine(path, currentDll);

			Console.WriteLine($"Switching to {desiredAuthor}'s wrapper...\n");

			if (File.Exists(D3D8) && File.Exists(desiredDllBackup))
			{
				string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;
				Console.WriteLine($"Found {Path.GetFileName(D3D8)} file by \"{D3D8Author}\"...");

				if (D3D8Author == currentAuthor)
				{
					if (File.Exists(currentDllBackup)) File.Delete(currentDllBackup);
					File.Move(D3D8, currentDllBackup);
					Console.WriteLine($"Renamed \"{Path.GetFileName(D3D8)}\" to \"{Path.GetFileName(currentDllBackup)}\"...");
					File.Move(desiredDllBackup, D3D8);
					Console.WriteLine($"Renamed \"{Path.GetFileName(desiredDllBackup)}\" to \"{Path.GetFileName(D3D8)}\"...");
					Console.WriteLine("Success.");
				}
			}
			else
			{
				if (!File.Exists(D3D8)) Console.WriteLine($"Error: \"{Path.GetFileName(D3D8)}\" is missing...");
				if (!File.Exists(desiredDllBackup)) Console.WriteLine($"Error: \"{Path.GetFileName(desiredDllBackup)}\" is missing...");
				utils.CloseInSeconds(4);
			}
		}

		public void D3DCRO()
		{
			SwitchD3D("crosire", "Dégé", "d3d8.cro", "d3d8.dgv");

			string DGV_CONF = Path.Combine(path, "dgVoodoo.conf");
			utils.ModifyDGVConfig(DGV_CONF, "DirectX", "DisableAndPassThru", "true");
		}

		public void D3DDGV()
		{
			SwitchD3D("Dégé", "crosire", "d3d8.dgv", "d3d8.cro");

			string DGV_CONF = Path.Combine(path, "dgVoodoo.conf");
			utils.ModifyDGVConfig(DGV_CONF, "DirectX", "DisableAndPassThru", "false");
		}

		public void DGV_TEXFILT(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "Filtering", setting);

		}

		public void DGV_AA(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "Antialiasing", setting);
		}

		public void DGV_VSYNC(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "ForceVerticalSync", setting);
		}

		public void DGV_FAKE(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "GeneralExt", "FullscreenAttributes", setting);
		}

		public void DGV_SAR(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "General", "ScalingMode", setting);
		}

		public void DGV_CAPMOUSE(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "General", "CaptureMouse", setting);
		}

		public void DGV_SCREENTOGGLE(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "DisableAltEnterToToggleScreenMode", setting);

			if (Convert.ToBoolean(setting))
			{
				utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "AppControlledScreenMode", setting);
				utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "General", "FullScreenMode", setting);
			}
		}

		public void DGV_DEFAULTWINDOWED(string setting)
		{
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "AppControlledScreenMode", setting);
			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "General", "FullScreenMode", setting);

			if (!Convert.ToBoolean(setting))
			{
				utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "DisableAltEnterToToggleScreenMode", setting);
			}
		}

		public void UPDATE(string url, string installDir)
		{
			Console.WriteLine("Waiting for wdlaunch to exit...");
			
			int timeout = 100; // 10 seconds
			while (Process.GetProcessesByName("wdlaunch").Length > 0 && timeout > 0)
			{
				Thread.Sleep(100);
				timeout--;
			}

			if (Process.GetProcessesByName("wdlaunch").Length > 0)
			{
				Console.WriteLine("Error: wdlaunch failed to close.");
				utils.CloseInSeconds(4);
				return;
			}

			Console.WriteLine($"Downloading update from: {url}");
			string tempFile = Path.Combine(Path.GetTempPath(), "wd_update.exe");

			if (File.Exists(tempFile)) File.Delete(tempFile);

			try
			{
				using (WebClient client = new WebClient())
				{
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
					
					client.DownloadProgressChanged += (s, e) =>
					{
						double bytesIn = double.Parse(e.BytesReceived.ToString());
						double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
						double percentage = bytesIn / totalBytes * 100;
						Console.Write($"\rDownloading update: {bytesIn / 1024 / 1024:0.00} MB / {totalBytes / 1024 / 1024:0.00} MB ({percentage:0}%)");
					};

					Console.WriteLine("Starting download...");
					// Use synchronous download wrapper to block but still get events
					var downloadTask = client.DownloadFileTaskAsync(new Uri(url), tempFile);
					downloadTask.Wait();
					Console.WriteLine("\nDownload complete.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"\nDownload failed: {e.Message}");
				utils.CloseInSeconds(4);
				return;
			}

			Console.WriteLine("Running installer...");
			try
			{
				string installArgs = "/S";
				if (!string.IsNullOrEmpty(installDir))
				{
					installArgs += $" /D={installDir}";
				}

				Process.Start(tempFile, installArgs);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to run installer: {e.Message}");
				utils.CloseInSeconds(4);
			}
		}

		public void PATCHFILES(bool KR)
		{
			DeleteFileIfExists("xdelta3.exe");
			DeleteFileIfExists("patchfiles.bat");
			WriteResourceToFile("xdelta3.exe", Resources.xdelta3);

			StringBuilder failedFiles = new StringBuilder();

			using (StreamWriter sw = new StreamWriter(Path.Combine(path, "patchfiles.bat")))
			{
				sw.WriteLine("@echo off");
				sw.WriteLine("echo.");

				foreach (string file in files)
				{
					string filePath = Path.Combine(path, file);

					DeleteFileIfExists(filePath + ".tmp");
					DeleteFileIfExists(filePath + ".vcdiff");

					string resourceName = "WDHelper.patches." + (KR ? "e2k" : "k2e") + "." + file + ".vcdiff";

					byte[] patchFile = utils.ExtractResource(resourceName);

					WriteResourceToFile(file + ".vcdiff", patchFile);

					sw.WriteLine($"echo Patching {file}...");
					sw.WriteLine($"xdelta3.exe -d -s \"{filePath}\" \"{filePath}.vcdiff\" \"{filePath}.tmp\"");
				}
				sw.WriteLine("exit");
			}

			Process.Start(Path.Combine(path, "patchfiles.bat"))?.WaitForExit();

			string[] bakNopFiles = { "whiteday121", "mod_beanbag102" };

			foreach (string file in files)
			{
				string filePath = Path.Combine(path, file);
				if (File.Exists(filePath + ".tmp"))
				{
					DeleteFileIfExists(file);
					File.Move(filePath + ".tmp", filePath);
				}
				else
				{
					failedFiles.AppendLine(file);
				}

				DeleteFileIfExists(filePath + ".vcdiff");
			}

			DeleteFileIfExists("xdelta3.exe");
			DeleteFileIfExists("patchfiles.bat");

			if (failedFiles.Length > 0)
			{
				Console.WriteLine("\n\nFailed to patch:\n" + failedFiles.ToString() + "\n\nPress enter to continue...");
				Console.ReadLine();
			}

			foreach (string file in bakNopFiles)
			{
				string bakFile = Path.Combine(path, $"{file}.bak");
				string nopFile = Path.Combine(path, $"{file}.nop");

				if (!KR)
				{
					if (File.Exists(nopFile)) File.Delete(bakFile);
					if (File.Exists(bakFile)) File.Move(bakFile, nopFile);
				}
				else
				{
					if (File.Exists(bakFile)) File.Delete(nopFile);
					if (File.Exists(nopFile)) File.Move(nopFile, bakFile);
				}
			}
		}

		private void DeleteFileIfExists(string fileName)
		{
			string filePath = Path.Combine(path, fileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		private void WriteResourceToFile(string fileName, byte[] resource)
		{
			if (resource == null) Console.WriteLine($"fileName == null");

			try
			{
				File.WriteAllBytes(Path.Combine(path, fileName), resource);
			}
			catch (Exception e)
			{
				Console.WriteLine($"\nError: {e.Message}");
			}
		}
	}

	public class Utils
	{
		public void CloseInSeconds(int seconds)
		{
			Console.Write("\n\nClosing in ");
			for (int i = 1; i <= seconds; i++)
			{
				Console.Write($"{i}...");
				System.Threading.Thread.Sleep(1000);
			}
		}
		public byte[] ExtractResource(String filename)
		{
			Assembly a = Assembly.GetExecutingAssembly();

			using (Stream resFilestream = a.GetManifestResourceStream(filename))
			{
				if (resFilestream == null)
				{
					return null;
				}

				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}

		}
		public void ModifyDGVConfig(string path, string heading, string parameter, string newValue)
		{
			try
			{
				// Read all lines from the file
				var lines = File.ReadAllLines(path).ToList();

				string currentHeading = null;  // Track the current heading

				// Find the line and replace the value
				for (int i = 0; i < lines.Count; i++)
				{
					var trimmedLine = lines[i].Trim();

					// Check if the line is a heading
					if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
					{
						currentHeading = trimmedLine.Trim('[', ']');
					}
					else if (currentHeading == heading && trimmedLine.StartsWith(parameter))
					{
						lines[i] = $"{parameter} = {newValue}";
					}
				}

				// Try to write the lines back to the file
				File.WriteAllLines(path, lines);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\nError: " + ex.Message);
				Console.ReadLine();
			}
		}
	}
}
