using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDLaunch.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

			//MoreSettingsTabControl.TabPages.Remove(MultiTab); // hidden for now

			PopulateJoinedNetworks();
		}

		public void SetText(bool KR)
		{
			// admin option tooltips
			hints.SetToolTip(PrivilegeIndicatorLabel, Resources.PrivilegeIndicatorTip + "\n\n" + Resources.AdminModeTip);
			hints.SetToolTip(RelaunchAsAdminButton, Resources.RelaunchAsAdminTip + "\n\n" + Resources.AdminModeTip);
			hints.SetToolTip(AlwaysAdminCheckBox, Resources.AlwaysAdminTip + "\n\n" + Resources.AdminModeTip);

			// admin options text
			RelaunchAsAdminButton.Text = Resources.RelaunchAsAdminTerm;
			AlwaysAdminCheckBox.Text = Resources.AlwaysAdminTerm;

			if (WDUtils.CheckAdmin())
			{
				PrivilegeIndicatorLabel.ForeColor = Color.Red;
				PrivilegeIndicatorLabel.Text = Resources.AdminModeTerm_MoreSettings;
			}
			else
			{
				PrivilegeIndicatorLabel.ForeColor = Color.Green;
				PrivilegeIndicatorLabel.Text = Resources.UserModeTerm;
			}

			// tab texts
			TweaksTab.Text = Resources.TweaksTerm;
			WrapperTab.Text = Resources.WrapperTerm;
			MultiTab.Text = Resources.OhJaemiTerm;

			// tweak tooltips
			hints.SetToolTip(NoJanitorCheckBox, Resources.NoJanitorTip);
			hints.SetToolTip(PeacefulJanitorCheckBox, Resources.PeacefulJanitorTip);
			hints.SetToolTip(NoFloatingHeadCheckBox, Resources.NoFloatingHeadTip);
			hints.SetToolTip(QuieterAlarmsCheckBox, Resources.QuieterAlarmsTip);
			hints.SetToolTip(NoHUDCheckBox, Resources.NoHUDTip);
			hints.SetToolTip(UnlockAllDifficultiesCheckBox, Resources.UnlockAllDifficultiesTip);
			hints.SetToolTip(UnlockAllOptionsCheckBox, Resources.UnlockAllOptionsTip);

			// tweaks text
			TweaksPanel1Label.Text = Resources.TweaksTerm;
			TweaksPanel2Label.Text = Resources.OptionsTerm;
			NoJanitorCheckBox.Text = Resources.NoJanitorTerm;
			PeacefulJanitorCheckBox.Text = Resources.PeacefulJanitorTerm;
			NoFloatingHeadCheckBox.Text = Resources.NoFloatingHeadTerm;
			QuieterAlarmsCheckBox.Text = Resources.QuieterAlarmsTerm;
			NoHUDCheckBox.Text = Resources.NoHUDTerm;
			UnlockAllDifficultiesCheckBox.Text = Resources.UnlockAllDifficultiesTerm;
			UnlockAllOptionsCheckBox.Text = Resources.UnlockAllOptionsTerm;

			// wrapper tooltips
			hints.SetToolTip(SelectWrapperLabel, Resources.SetWrapperTip);
			hints.SetToolTip(DGVRadioButton, Resources.DGVTip);
			hints.SetToolTip(CRORadioButton, Resources.CROTip);
			hints.SetToolTip(AAComboBox, Resources.AATip);
			hints.SetToolTip(AAComboBoxLabel, Resources.AATip);
			hints.SetToolTip(TexFiltComboBox, Resources.TexFiltTip);
			hints.SetToolTip(TexFiltComboBoxLabel, Resources.TexFiltTip);
			hints.SetToolTip(VSyncCheckBox, Resources.VSyncTip);
			hints.SetToolTip(MonitorSelectNumBox, Resources.MonitorSelectTip);
			hints.SetToolTip(MonitorSelectLabel, Resources.MonitorSelectTip);
			hints.SetToolTip(FakeFullscreenAttrCheckBox, Resources.FakeFullscreenAttrTip);
			hints.SetToolTip(StretchedARScalingCheckBox, Resources.StretchedARScalingTip);
			hints.SetToolTip(CaptureMouseCheckBox, Resources.CaptureMouseTip);
			hints.SetToolTip(ToggleScreenModeCheckBox, Resources.ToggleScreenModeTip);
			hints.SetToolTip(DefaultWindowedCheckBox, Resources.DefaultWindowedTip);

			// wrapper texts
			SelectWrapperLabel.Text = Resources.SelectWrapperTerm;
			AAComboBoxLabel.Text = Resources.AATerm;
			TexFiltComboBoxLabel.Text = Resources.TexFiltTerm;
			MonitorSelectLabel.Text = Resources.MonitorTerm;
			FakeFullscreenAttrCheckBox.Text = Resources.FakeFullscreenAttrTerm;
			StretchedARScalingCheckBox.Text = Resources.StretchedARScalingTerm;
			CaptureMouseCheckBox.Text = Resources.CaptureMouseTerm;
			ToggleScreenModeCheckBox.Text = Resources.ToggleScreenModeTerm;
			DefaultWindowedCheckBox.Text = Resources.DefaultWindowedTerm;

			// oh jaemi (multiplayer) tooltips
			hints.SetToolTip(InstallLaunchZTButton, Resources.InstallLaunchZTTip);
			hints.SetToolTip(ZTRunningLabel, Resources.ZTRunningTip);
			hints.SetToolTip(ZTJoinNetworkButton, Resources.ZTJoinNetworkTip);
			hints.SetToolTip(ZTLeaveNetworkButton, Resources.ZTLeaveNetworkTip);
			hints.SetToolTip(ZTCreateNetworkButton, Resources.ZTHostNetworkTip);
			hints.SetToolTip(SetZTAPIButton, Resources.SetZTAPITip);
			hints.SetToolTip(ZTAPIConnectedLabel, Resources.ZTAPIConnectedTip);
			hints.SetToolTip(ZTHostNetworkButton, Resources.ZTHostNetworkTip);
			hints.SetToolTip(ZTRemoveNetworkButton, Resources.ZTRemoveNetworkTip);

			// oh jaemi (multiplayer) texts
			InstallLaunchZTButton.Text = Resources.InstallLaunchZTTerm;
			//ZTRunningLabel.Text = Resources.ZTRunningTerm + Resources.NoTerm;
			//ZTRunningLabel.Text = Resources.ZTRunningTerm + Resources.YesTerm;
			ZTJoinNetworkButton.Text = Resources.JoinTerm;
			ZTLeaveNetworkButton.Text = Resources.LeaveTerm;
			ZTCreateNetworkButton.Text = Resources.CreateTerm;

			OhJaemiHostingPanelLabel.Text = Resources.HostingTerm;
			SetZTAPIButton.Text = Resources.SetZTAPITerm;
			//ZTAPIConnectedLabel.Text = Resources.ZTAPIConnectedTerm + Resources.NoTerm;
			//ZTAPIConnectedLabel.Text = Resources.ZTAPIConnectedTerm + Resources.YesTerm;
			ZTHostNetworkButton.Text = Resources.HostTerm;
			ZTRemoveNetworkButton.Text = Resources.DeleteTerm;
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
			Registry.SetValue(regPath, "CostumeShow", "0");
			Registry.SetValue(regPath, "PatrolManChange", "0");
			mainForm.SetUIValues();
		}

		private void PopulateJoinedNetworks()
		{
			ZTJoinedNetworksListView.Items.Clear();
		
			List<ListViewItem> items = OJZT.JoinedNetworks();
			foreach (ListViewItem item in items)
			{
				ZTJoinedNetworksListView.Items.Add(item);
			}
		}

		private void ZTCreateNetworkButton_Click(object sender, EventArgs e)
		{
			Process.Start("https://my.zerotier.com/");
		}

		private void ZTLeaveNetworkButton_Click(object sender, EventArgs e)
		{
			mainForm.TopMost = this.TopMost = false;

			if (ZTJoinedNetworksListView.SelectedItems.Count > 0)
			{
				string selectedNetworkID = ZTJoinedNetworksListView.SelectedItems[0].Text;
				OJZT.LeaveNetwork(selectedNetworkID);
				PopulateJoinedNetworks();
			}
			else
			{
				MessageBox.Show("Select a network to leave.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			mainForm.TopMost = this.TopMost = true;
		}

		private void ZTJoinNetworkButton_Click(object sender, EventArgs e)
		{
			mainForm.TopMost = this.TopMost = false;

			// Get the network ID from the user
			string networkId = Interaction.InputBox("Enter the network ID to join:", "Join Network");

			OJZT.JoinNetwork(networkId);

			PopulateJoinedNetworks();

			mainForm.TopMost = this.TopMost = true;
		}

		private async void InstallLaunchZTButton_Click(object sender, EventArgs e)
		{
			mainForm.TopMost = this.TopMost = false;

			await OJZT.InstallOrStartZeroTier();
			PopulateJoinedNetworks();

			mainForm.TopMost = this.TopMost = true;
		}
	}
}
