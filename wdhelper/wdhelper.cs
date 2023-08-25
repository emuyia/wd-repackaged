using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using WDHelper.Properties;
using System.Text;
using System.Linq;

namespace WDHelper
{
    class WDHelper
    {
        static void Main(string[] args)
        {
			//Console.WriteLine("\n===== WDHelper =====\n");
			//Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(" + string.Join(", ", args.Select(a => $"\"{a}\"")) + ")");

			string arg1 = args.Length > 1 ? args[1] : "";
			string arg2 = args.Length > 2 ? args[2] : "";

			//Console.WriteLine($"arg={arg1}");


			AdminTasks adminTasks = new AdminTasks();

            if (args.Length > 0)
            {
				adminTasks.SetPath(args[0]);
            }

            try
            {
                switch (arg1)
                {
                    case "ENABLED3D8":
						adminTasks.ENABLED3D8();
						break;
					case "DISABLED3D8":
						adminTasks.DISABLED3D8();
                        break;
					case "LANG_KR":
						adminTasks.PATCHFILES(true);
                        break;
					case "LANG_EN":
						adminTasks.PATCHFILES(false);
                        break;
					case "WDLAUNCH":
						adminTasks.WDLAUNCH();
						break;
                    case "AA":
                        adminTasks.AA(arg2);
                        break;
                    case "TEXFILT":
                        adminTasks.TEXFILT(arg2);
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
			//Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({pathArg})");

			path = pathArg;
        }

        public void TEXFILT(string setting)
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}(setting = {setting})");

			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "Filtering", setting);

		}

		public void AA(string setting)
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}(setting = {setting})");

			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "Antialiasing", setting);
		}

		public void WDLAUNCH()
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}()");

			Process.Start(Path.Combine(path, "wdlaunch.exe"));
		}

		public void ENABLED3D8()
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}()");

			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "DisableAndPassThru", "false");
		}

		public void DISABLED3D8()
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}()");

			utils.ModifyDGVConfig(Path.Combine(path, "dgVoodoo.conf"), "DirectX", "DisableAndPassThru", "true");
		}

        public void PATCHFILES(bool KR)
        {
			//Console.WriteLine($"> {MethodBase.GetCurrentMethod().Name}(KR = {KR})");

			DeleteFileIfExists("xdelta3.exe");
            DeleteFileIfExists("patchfiles.bat");
            WriteResourceToFile("xdelta3.exe", Resources.xdelta3);

            StringBuilder failedFiles = new StringBuilder();

			//Console.WriteLine("\nCreating patchfiles.bat...");
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

                    //Console.WriteLine($"\nFetching patch file: \"{resourceName}\"");
                    byte[] patchFile = utils.ExtractResource(resourceName);

                    WriteResourceToFile(file + ".vcdiff", patchFile);

                    sw.WriteLine($"echo Patching {file}...");
                    sw.WriteLine($"xdelta3.exe -d -s \"{filePath}\" \"{filePath}.vcdiff\" \"{filePath}.tmp\"");

					//Console.Write(".");
				}

                //sw.WriteLine("timeout 1 >nul");
                sw.WriteLine("exit");
            }

            //Console.WriteLine("\nRunning patchfiles.bat...");
            //Console.Write("> Patching files...");
			Process.Start(Path.Combine(path, "patchfiles.bat"))?.WaitForExit();

            //string[] bakNopFiles = { "whiteday121", "whiteday120", "whiteday119", "mod_beanbag102" };
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

				//Console.Write(".");
			}

            DeleteFileIfExists("xdelta3.exe");
            DeleteFileIfExists("patchfiles.bat");

            if (failedFiles.Length > 0)
            {
                Console.WriteLine("\n\nFailed to patch:\n" + failedFiles.ToString() + "\n\nPress enter to continue...");
                Console.ReadLine();
            }

            //Console.Write(".");

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
				//Console.Write(".");
			}

			//System.Threading.Thread.Sleep(500);
		}

        private void DeleteFileIfExists(string fileName)
        {
			string filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath))
            {
				//Console.WriteLine($"Found file: \"{fileName}\", deleting...");
				File.Delete(filePath);
			}
        }

        private void WriteResourceToFile(string fileName, byte[] resource)
        {
			//Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({fileName})");

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

    public class Utils {
        public byte[] ExtractResource(String filename)
        {
			//Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({filename})");

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
			//Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({path}, {heading}, {parameter}, {newValue})");

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

				//Console.WriteLine($"File.WriteAllLines({path}, lines[])");

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
