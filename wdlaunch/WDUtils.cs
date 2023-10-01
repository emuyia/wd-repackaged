using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

namespace WDLaunch
{
	public static class WDUtils
	{
		public static string Dir = Directory.GetCurrentDirectory();
		public static string WDLaunchPath = $"\"{Path.Combine(Dir, "wdlaunch.exe")}\"";
		public static string DGVConfPath = Path.Combine(Dir, "dgVoodoo.conf");

		public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({dirPath}, throwIfFails = {throwIfFails})");

			try
			{
				using (FileStream fs = File.Create(
					Path.Combine(
						dirPath,
						Path.GetRandomFileName()
					),
					1,
					FileOptions.DeleteOnClose)
				)
				{ }
				return true;
			}
			catch
			{
				if (throwIfFails)
					throw;
				else
					return false;
			}
		}

		public static bool CheckAdmin()
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static bool NewProcess(string path, string args, bool admin)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(path = {path}, args = {args}, admin = {admin})");

			var p = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = path,
					Arguments = $" {args}",
					Verb = admin ? "runas" : ""
				}
			};

			try
			{
				p.Start();
			}
			catch (Exception e)
			{
				MessageBox.Show($"Failed to run \"{path}\"{(admin ? " with admin privileges" : "")}.\n\nError: {e.Message}", "",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			return true;
		}

		public static void OpenVirtualDir(string folder) // opens VirtualStore directory if necessary
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(folder = {folder})");

			if (CheckAdmin() || IsDirectoryWritable(Dir))
			{
				if (!Directory.Exists(Dir + folder))
					Directory.CreateDirectory(Dir + folder);

				Process.Start(Dir + folder);
			}
			else
			{
				string virtualStorePath = Environment.GetEnvironmentVariable("LocalAppData") + @"\VirtualStore";
				string trimmedDir = @"\" + Dir.Substring(Path.GetPathRoot(Dir).Length);

				if (!Directory.Exists(virtualStorePath + trimmedDir + folder))
					Directory.CreateDirectory(virtualStorePath + trimmedDir + folder);

				Process.Start(virtualStorePath + trimmedDir + folder);
			}
		}
		public static void CreateRegistryIfNotExist(string path, string valueName, string value)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(path = {path}, valueName = {valueName}, value = {value})");

			if (Registry.GetValue(path, valueName, null) == null) Registry.SetValue(path, valueName, value);
		}
		public static void WDHelper(string task, string arg = "", string arg2 = "")
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(task = {task})");

			try
			{
				string wdhelperPath = Path.GetTempPath() + "wdhelper.exe";

				Process wdhelper = new Process();

				ProcessStartInfo wdhelperProcessInfo = new ProcessStartInfo
				{
					FileName = wdhelperPath,
					Verb = "runas",
					Arguments = $"\"{Dir}\" \"{task}\" \"{arg}\" \"{arg2}\""
				};

				wdhelper.StartInfo = wdhelperProcessInfo;

				if (!File.Exists(wdhelperPath)) File.WriteAllBytes(wdhelperPath, Properties.Resources.wdhelper);
				wdhelper.Start();
				wdhelper.WaitForExit();
				if (File.Exists(wdhelperPath)) File.Delete(wdhelperPath);
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error during \"WDUtils.WDHelper({task})\":\n\n{e.Message}");
			}
		}

		public static string ReadDGVConfig(string path, string heading, string parameter)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(path = {path}, heading = {heading}, parameter = {parameter})");

			try
			{
				// Read all lines from the file
				var lines = File.ReadAllLines(path).ToList();

				string currentHeading = null;  // Track the current heading

				// Find the line and extract the value
				foreach (string line in lines)
				{
					var trimmedLine = line.Trim();

					// Check if the line is a heading
					if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
					{
						currentHeading = trimmedLine.Trim('[', ']');
					}
					else if (currentHeading == heading && trimmedLine.StartsWith(parameter))
					{
						var parts = trimmedLine.Split('=');

						if (parts.Length > 1)
						{
							Console.WriteLine($"	[{heading}] {parameter} = \"{parts[1].Trim()}\"");
							return parts[1].Trim();  // Return the value part
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error in {MethodBase.GetCurrentMethod().Name}: {ex.Message}");
			}

			return null;  // Return null if the parameter was not found
		}

		public static Bitmap ResizeImage(Image image, int width)
		{
			// Calculate new height based on original aspect ratio
			int height = (int)(width / (float)image.Width * image.Height);

			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingMode = CompositingMode.SourceCopy;

				graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
			}

			return destImage;
		}
	}
}
