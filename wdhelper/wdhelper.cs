using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WDHelper.Properties;

namespace WDHelper
{
	class WDHelper
	{
		static void Main(string[] args)
		{
			string arg1 = args.Length > 1 ? args[1] : "";
			string arg2 = args.Length > 2 ? args[2] : "";

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
						adminTasks.WRAPD3D(arg2);
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

		public void WRAPD3D(string setting)
		{
			bool settingBool = Convert.ToBoolean(setting);

			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "DisableAndPassThru", (!settingBool).ToString());

			string D3D8 = Path.Combine(path, "d3d8.dll");
			string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;

			string dgv_backup = Path.Combine(path, "d3d8.dgv");
			string cro_backup = Path.Combine(path, "d3d8.cro");

			if (D3D8Author == "Dégé")
			{
				File.Move(D3D8, dgv_backup);
			}
			else
			{
				File.Move(D3D8, cro_backup);
			}
		}

		public void D3DCRO()
		{
			string D3D8 = Path.Combine(path, "d3d8.dll");
			string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;

			string dgv_backup = Path.Combine(path, "d3d8.dgv");
			string cro_backup = Path.Combine(path, "d3d8.cro");

			if (D3D8Author == "Dégé")
			{
				File.Move(D3D8, dgv_backup);
				File.Move(cro_backup, D3D8);
			}
		}

		public void D3DDGV()
		{
			string D3D8 = Path.Combine(path, "d3d8.dll");
			string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;

			if (D3D8Author == "crosire")
			{
				string dgv_backup = Path.Combine(path, "d3d8.dgv");
				string cro_backup = Path.Combine(path, "d3d8.cro");

				File.Move(D3D8, cro_backup);
				File.Move(dgv_backup, D3D8);
			}
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
