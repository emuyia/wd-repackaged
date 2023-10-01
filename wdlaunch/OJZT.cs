using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WDLaunch
{
	public static class OJZT
	{
		public static List<ListViewItem> JoinedNetworks()
		{
			// The location of the networks.d directory
			string programData = Environment.GetEnvironmentVariable("ProgramData");
			string networksDir = Path.Combine(programData, @"ZeroTier\One\networks.d");

			// Refresh the directory before getting the files
			DirectoryInfo networksDirInfo = new DirectoryInfo(networksDir);
			networksDirInfo.Refresh();

			// Get all .conf files in the directory, excluding .local.conf files
			string[] networkFiles = Directory.GetFiles(networksDir, "*.conf")
				.Where(f => !f.Contains(".local")).ToArray();

			List<ListViewItem> items = new List<ListViewItem>();

			// Iterate over the network files
			foreach (string networkFile in networkFiles)
			{
				// The network ID is the filename without the extension
				string networkId = Path.GetFileNameWithoutExtension(networkFile);

				// Read the network name from the .conf file
				string networkName = File.ReadLines(networkFile)
					.FirstOrDefault(line => line.StartsWith("n="));
				if (networkName != null)
				{
					// Remove the "n=" prefix from the network name
					networkName = networkName.Substring(2);
				}

				ListViewItem item = new ListViewItem(new string[] { networkId, networkName });
				items.Add(item);
			}

			return items;
		}

		public static void LeaveNetwork(string networkId)
		{
			// Construct the path to the ZeroTier executable
			string programData = Environment.GetEnvironmentVariable("ProgramData");
			string executablePath = Path.Combine(programData, @"ZeroTier\One\zerotier-one_x64.exe");

			// Construct the full command to run zerotier-one_x64.exe
			string command = $"{executablePath} -q leave {networkId}";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/C {command}",
				Verb = "runas", // Run as administrator
				UseShellExecute = true,
				CreateNoWindow = true,
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();
			process.WaitForExit();
		}

		public static void JoinNetwork(string networkId)
		{
			if (string.IsNullOrEmpty(networkId))
			{
				// The user clicked Cancel or entered an empty network ID
				return;
			}

			// Construct the path to the ZeroTier executable
			string programData = Environment.GetEnvironmentVariable("ProgramData");
			string executablePath = Path.Combine(programData, @"ZeroTier\One\zerotier-one_x64.exe");

			// Construct the full command to run zerotier-one_x64.exe
			string command = $"{executablePath} -q join {networkId}";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/C {command}",
				Verb = "runas", // Run as administrator
				UseShellExecute = true,
				CreateNoWindow = true,
			};

			Process process = new Process { StartInfo = startInfo };
			process.Start();
			process.WaitForExit();
		}

		public static async Task InstallOrStartZeroTier()
		{
			// Define the expected path of ZeroTier executable
			string programData = Environment.GetEnvironmentVariable("ProgramData");
			string executablePath = Path.Combine(programData, @"ZeroTier\One\zerotier-one_x64.exe");

			// Check if the ZeroTier executable exists
			if (File.Exists(executablePath))
			{
				// Start ZeroTier
				string programFiles32 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				string ZTdesktop = Path.Combine(programFiles32, @"ZeroTier\One\zerotier_desktop_ui.exe");
				Process.Start(ZTdesktop);
			}
			else
			{
				// Define the URL of the ZeroTier installer
				string installerUrl = "https://download.zerotier.com/dist/ZeroTier%20One.msi";

				// Define the path to save the ZeroTier installer
				string installerPath = Path.Combine(Path.GetTempPath(), "ZeroTierOne.msi");

				// Create a new WebClient instance
				using (WebClient client = new WebClient())
				{
					// Create a new Form with a ProgressBar and a Label
					Form form = new Form { Size = new Size(400, 100),
										   ShowIcon = false,
										   StartPosition = FormStartPosition.CenterScreen,
										   ControlBox = false,
										   SizeGripStyle = SizeGripStyle.Hide };

					ProgressBar progressBar = new ProgressBar { Dock = DockStyle.Bottom, Maximum = 100 };
					Label label = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
					form.Controls.Add(label);
					form.Controls.Add(progressBar);

					// Hook up a handler to the WebClient.DownloadProgressChanged event
					client.DownloadProgressChanged += (sender, e) =>
					{
						// Update the ProgressBar and Label on the UI thread
						progressBar.Value = e.ProgressPercentage;
						label.Text = $"Download progress: {e.ProgressPercentage}%";
					};

					// Show the Form
					form.Show();

					// Start the download task
					await client.DownloadFileTaskAsync(new Uri(installerUrl), installerPath);

					// Close the Form
					form.Close();
				}

				// Start the ZeroTier installer
				Process installerProcess = Process.Start(installerPath);
				installerProcess.WaitForExit();
			}
		}
	}
}
