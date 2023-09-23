using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WDLaunch.Properties;

namespace WDLaunch
{
	public partial class WDLaunchSettings_Form : Form
	{
		private WDLaunch_Form mainForm;
		private const string regPath = @"HKEY_CURRENT_USER\SOFTWARE\Sonnori\WhiteDay\Option";

		public WDLaunchSettings_Form(WDLaunch_Form mainForm)
		{
			InitializeComponent();

			this.BackColor = Color.Magenta;
			this.TransparencyKey = Color.Magenta;

			this.mainForm = mainForm;
		}

		private void WDLaunchSettings_Form_Load(object sender, EventArgs e)
		{
			// Remove window border
			FormBorderStyle = FormBorderStyle.None;

			MoreSettingsTabControl.TabPages.Remove(MultiTab); // hidden for now
		}

		public void SetText(bool KR)
		{
			hints.SetToolTip(AAComboBox, Resources.AATip);
			hints.SetToolTip(TexFiltComboBox, Resources.TexFiltTip);
		}

		public void UpdateLocation(Point newLocation)
		{
			this.Location = newLocation;
		}

		private void AAComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_AA", aaMapToTech[AAComboBox.Text]);

			mainForm.SetUIValues();
		}

		private void TexFiltComboBox_SelectionChangeCommitted(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_TEXFILT", texFiltMapToTech[TexFiltComboBox.Text]);

			mainForm.SetUIValues();
		}

		public Dictionary<string, string> aaMapToTech = new Dictionary<string, string>()
		{
			{"Native", "appdriven"},
			{"Off", "off"},
			{"2x MSAA", "2x"},
			{"4x MSAA", "4x"},
			{"8x MSAA", "8x"}
		};

		public Dictionary<string, string> texFiltMapToTech = new Dictionary<string, string>()
		{
			{"Native", "appdriven"},
			{"Bilinear", "bilinear"},
			{"Trilinear", "trilinear"},
			{"Anisotropic 2x", "2"},
			{"Anisotropic 4x", "4"},
			{"Anisotropic 8x", "8"},
			{"Anisotropic 16x", "16"}
		};

		public Dictionary<string, string> aaMapToUI;
		public Dictionary<string, string> texFiltMapToUI;

		private void FakeFullscreenAttrCheckBox_Click(object sender, EventArgs e)
		{
			if (FakeFullscreenAttrCheckBox.Checked)
			{
				WDUtils.WDHelper("DGV_FAKE", "Fake");
			}
			else
			{
				WDUtils.WDHelper("DGV_FAKE", "");
			}

			mainForm.SetUIValues();
		}

		private void StretchedARScalingCheckBox_Click(object sender, EventArgs e)
		{
			if (StretchedARScalingCheckBox.Checked)
			{
				WDUtils.WDHelper("DGV_SAR", "stretched_ar");
			}
			else
			{
				WDUtils.WDHelper("DGV_SAR", "unspecified");
			}

			mainForm.SetUIValues();
		}

		private void VSyncCheckBox_Click(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_VSYNC", VSyncCheckBox.Checked.ToString());
			mainForm.SetUIValues();
		}

		private void CaptureMouseCheckBox_Click(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_CAPMOUSE", CaptureMouseCheckBox.Checked.ToString());
			mainForm.SetUIValues();
		}

		private void ToggleScreenModeCheckBox_Click(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_SCREENTOGGLE", (!ToggleScreenModeCheckBox.Checked).ToString());
			mainForm.SetUIValues();
		}

		private void DefaultWindowedCheckBox_Click(object sender, EventArgs e)
		{
			WDUtils.WDHelper("DGV_DEFAULTWINDOWED", (!DefaultWindowedCheckBox.Checked).ToString());
			mainForm.SetUIValues();
		}

		private void RelaunchAsAdminButton_Click(object sender, EventArgs e)
		{
			WDUtils.WDHelper("WDLAUNCH");
			Application.Exit();
			Environment.Exit(0);
		}

		private void AlwaysAdminCheckBox_Click(object sender, EventArgs e)
		{
			Settings.Default.AlwaysAdmin = !Settings.Default.AlwaysAdmin;
			Settings.Default.Save();
			mainForm.SetUIValues();
		}

		private void SwitchD3DWrapper(string desiredAuthor, string helperArg, string wrapperValue)
		{
			string D3D8 = Path.Combine(WDUtils.Dir, "d3d8.dll");

			if (!File.Exists(D3D8))
			{
				MessageBox.Show($"Error: \"{D3D8}\" is missing...");
				mainForm.SetUIValues();
				return;
			}

			string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;

			Console.WriteLine($"D3D8Author: {D3D8Author}");

			if (D3D8Author != desiredAuthor)
			{
				Console.WriteLine($"Starting WDHelper(\"{helperArg}\")");
				WDUtils.WDHelper(helperArg);

				if (FileVersionInfo.GetVersionInfo(D3D8).CompanyName == desiredAuthor)
				{
					Settings.Default.Wrapper = wrapperValue;
					Settings.Default.Save();
					Console.WriteLine($"D3D8Author is now: {FileVersionInfo.GetVersionInfo(D3D8).CompanyName}");
				}
			}
			mainForm.SetUIValues();
		}

		private void DGVRadioButton_Click(object sender, EventArgs e)
		{
			SwitchD3DWrapper("Dégé", "D3DDGV", "DGV");
		}

		private void CRORadioButton_Click(object sender, EventArgs e)
		{
			SwitchD3DWrapper("crosire", "D3DCRO", "CRO");
		}

		private void NoJanitorCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "NoJanitor", !NoJanitorCheckBox.Checked);
			mainForm.SetUIValues();
		}

		private void PeacefulJanitorCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "PeacefulJanitor", !PeacefulJanitorCheckBox.Checked);
			mainForm.SetUIValues();
		}

		private void NoFloatingHeadCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "NoFloatingHead", !NoFloatingHeadCheckBox.Checked);
			mainForm.SetUIValues();
		}

		private void QuieterAlarmsCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "QuieterAlarms", !QuieterAlarmsCheckBox.Checked);
			mainForm.SetUIValues();
		}

		private void NoHUDCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "NoHUD", !NoHUDCheckBox.Checked);
			mainForm.SetUIValues();
		}

		private void UnlockAllDifficultiesCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "NowComplete", !UnlockAllDifficultiesCheckBox.Checked ? "111 93 160" : "");
			mainForm.SetUIValues();
		}

		private void UnlockAllOptionsCheckBox_Click(object sender, EventArgs e)
		{
			Registry.SetValue(regPath, "CostumeChange", !UnlockAllOptionsCheckBox.Checked ? "113 93 150" : "");
			Registry.SetValue(regPath, "PatrolManPlay", !UnlockAllOptionsCheckBox.Checked ? "113 93 126" : "");
			mainForm.SetUIValues();
		}
	}
}
