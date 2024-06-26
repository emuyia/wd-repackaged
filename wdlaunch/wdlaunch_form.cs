﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WDLaunch.Properties;

namespace WDLaunch
{
	public partial class WDLaunch_Form : Form
	{
		private const string regPath = @"HKEY_CURRENT_USER\SOFTWARE\Sonnori\WhiteDay";

		private string DIR { get; set; }

		readonly WDLaunchSettings_Form settingsForm;

		Image LaunchButtonImage;
		Image LaunchButtonHoverImage;
		Image OhJaemiLaunchButtonImage;
		Image OhJaemiLaunchButtonHoverImage;
		Image ExitButtonImage;
		Image ExitButtonHoverImage;
		Image WDBgImage;
		Image OJBgImage;

		public WDLaunch_Form(string[] args)
		{
			InitializeComponent();

			if (Settings.Default.AlwaysAdmin && !WDUtils.CheckAdmin() && !((Control.ModifierKeys & Keys.Shift) == Keys.Shift))
			{
				WDUtils.WDHelper("WDLAUNCH");
				Application.Exit();
				Environment.Exit(0);
			}

			settingsForm = new WDLaunchSettings_Form(this)
			{
				Visible = false,
			};

			Settings.Default.Reload();

			// Initialise hover effects for buttons
			InitMouseOverButtons();

			DIR = Directory.GetCurrentDirectory();

			settingsForm.aaMapToUI = settingsForm.aaMapToTech.ToDictionary(x => x.Value, x => x.Key);
			settingsForm.texFiltMapToUI = settingsForm.texFiltMapToTech.ToDictionary(x => x.Value, x => x.Key);

			WDBgImage = WDUtils.ResizeImage(Resources.wdbg, this.Width);
			OJBgImage = WDUtils.ResizeImage(Resources.ojbg, this.Width);
			this.BackgroundImage = WDBgImage;

			ExitButtonImage = WDUtils.ResizeImage(Resources._140, ExitButton.Width);
			ExitButtonHoverImage = WDUtils.ResizeImage(Resources._140_hover, ExitButton.Width);
			ExitButton.Image = ExitButtonImage;
		}

		private void WDLaunch_Load(object sender, EventArgs e)
		{
			this.TopMost = settingsForm.TopMost = false;

			// Set UI values
			SetUIValues();

			// Do registry tasks
			HandleRegistryTasks();

			// Get version number from registry
			GetVersionFromRegistry();

			// Remove window border
			FormBorderStyle = FormBorderStyle.None;

			// Initialise location of additional settings form
			settingsForm.Location = CalculateSettingsFormLocation();

			this.TopMost = settingsForm.TopMost = true;
		}

		// Initialisation
		public void InitMouseOverButtons()
		{
			LaunchButton.MouseEnter += Button_MouseEnter;
			LaunchButton.MouseLeave += Button_MouseLeave;
			OhJaemiLaunchButton.MouseLeave += Button_MouseEnter;
			OhJaemiLaunchButton.MouseLeave += Button_MouseLeave;
			ExitButton.MouseEnter += Button_MouseEnter;
			ExitButton.MouseLeave += Button_MouseLeave;
			OpenMainDirButton.MouseEnter += Button_MouseEnter;
			OpenMainDirButton.MouseLeave += Button_MouseLeave;
			OpenSavesDirButton.MouseEnter += Button_MouseEnter;
			OpenSavesDirButton.MouseLeave += Button_MouseLeave;
			OpenCaptureDirButton.MouseEnter += Button_MouseEnter;
			OpenCaptureDirButton.MouseLeave += Button_MouseLeave;
		}

		public void SetUIValues()
		{
			AutoLaunchCheckBox.Checked = IsDefaultDeviceKeyExists() && Settings.Default.AutoLaunch;
			FixLocaleCheckBox.Checked = Settings.Default.UseLocaleEmulator;
			settingsForm.AlwaysAdminCheckBox.Checked = Settings.Default.AlwaysAdmin;
			AdminModeLabel.Visible = WDUtils.CheckAdmin();

			settingsForm.NoJanitorCheckBox.Checked = Convert.ToBoolean(Registry.GetValue(regPath + @"\Option", "NoJanitor", "false"));
			settingsForm.PeacefulJanitorCheckBox.Checked = Convert.ToBoolean(Registry.GetValue(regPath + @"\Option", "PeacefulJanitor", "false"));
			settingsForm.NoFloatingHeadCheckBox.Checked = Convert.ToBoolean(Registry.GetValue(regPath + @"\Option", "NoFloatingHead", "false"));
			settingsForm.NoHUDCheckBox.Checked = Convert.ToBoolean(Registry.GetValue(regPath + @"\Option", "NoHUD", "false"));
			settingsForm.QuieterAlarmsCheckBox.Checked = Convert.ToBoolean(Registry.GetValue(regPath + @"\Option", "QuieterAlarms", "false"));
			settingsForm.UnlockAllDifficultiesCheckBox.Checked = Registry.GetValue(regPath + @"\Option", "NowComplete", "").ToString() == "111 93 160";
			settingsForm.UnlockAllOptionsCheckBox.Checked =
				Registry.GetValue(regPath + @"\Option", "CostumeChange", "").ToString() == "113 93 150" &&
				Registry.GetValue(regPath + @"\Option", "PatrolManPlay", "").ToString() == "113 93 126";

			string DGV_CONF = $"{DIR}\\dgVoodoo.conf";
			string D3D8 = Path.Combine(DIR, "d3d8.dll");
			bool DGV_DirectX_Disabled;

			if (!File.Exists(DGV_CONF))
			{
				MessageBox.Show($"Could not find \"{Path.GetFileName(DGV_CONF)}\".");
				if (File.Exists(D3D8))
				{
					D3D8WrapperCheckBox.Checked = File.Exists(D3D8);
					string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;
					settingsForm.DGVRadioButton.Enabled = true;
					settingsForm.CRORadioButton.Enabled = true;
					if (D3D8Author == "Dégé")
					{
						settingsForm.DGVRadioButton.Checked = true;
						settingsForm.CRORadioButton.Checked = false;
					}
					else
					{
						settingsForm.DGVRadioButton.Checked = false;
						settingsForm.CRORadioButton.Checked = true;
					}
					SetDGVControlState("Checked", false);
					SetDGVControlState("Enabled", false);
					return;
				}
			}

			DGV_DirectX_Disabled = Convert.ToBoolean(WDUtils.ReadDGVConfig(DGV_CONF, "DirectX", "DisableAndPassThru"));

			D3D8WrapperCheckBox.Checked = (!DGV_DirectX_Disabled && File.Exists(D3D8)) || File.Exists(D3D8);

			if (D3D8WrapperCheckBox.Checked)
			{
				settingsForm.DGVRadioButton.Enabled = true;
				settingsForm.CRORadioButton.Enabled = true;

				string D3D8Author = FileVersionInfo.GetVersionInfo(D3D8).CompanyName;

				if (D3D8Author == "Dégé") { // dgVoodoo wrapper
					SetDGVControlState("Enabled", true);

					settingsForm.DGVRadioButton.Checked = true;
					settingsForm.CRORadioButton.Checked = false;

					Settings.Default.Wrapper = "DGV";

					// Get DGV Settings Values
					settingsForm.VSyncCheckBox.Checked = Convert.ToBoolean(
						WDUtils.ReadDGVConfig(DGV_CONF, "DirectX", "ForceVerticalSync"));

					settingsForm.FakeFullscreenAttrCheckBox.Checked =
						WDUtils.ReadDGVConfig(DGV_CONF, "GeneralExt", "FullscreenAttributes") == "Fake";

					settingsForm.StretchedARScalingCheckBox.Checked =
						WDUtils.ReadDGVConfig(DGV_CONF, "General", "ScalingMode") == "stretched_ar";

					settingsForm.CaptureMouseCheckBox.Checked = Convert.ToBoolean(
						WDUtils.ReadDGVConfig(DGV_CONF, "General", "CaptureMouse"));

					// Windowed Mode Options
					settingsForm.ToggleScreenModeCheckBox.Checked = !Convert.ToBoolean(
						WDUtils.ReadDGVConfig(DGV_CONF, "DirectX", "DisableAltEnterToToggleScreenMode"));

					if (settingsForm.ToggleScreenModeCheckBox.Checked)
					{
						settingsForm.DefaultWindowedCheckBox.Enabled = true;
						settingsForm.DefaultWindowedCheckBox.Checked =
							!Convert.ToBoolean(WDUtils.ReadDGVConfig(DGV_CONF, "General", "FullScreenMode")) &&
							!Convert.ToBoolean(WDUtils.ReadDGVConfig(DGV_CONF, "DirectX", "AppControlledScreenMode")) &&
							!Convert.ToBoolean(WDUtils.ReadDGVConfig(DGV_CONF, "DirectX", "DisableAltEnterToToggleScreenMode"));
					}
					else
					{
						settingsForm.DefaultWindowedCheckBox.Enabled = false;
						settingsForm.DefaultWindowedCheckBox.Checked = false;
					}

					// AA & TextFilt Options
					string aaOption = WDUtils.ReadDGVConfig(
						WDUtils.DGVConfPath, "DirectX", "Antialiasing");

					string texfiltOption = WDUtils.ReadDGVConfig(
						WDUtils.DGVConfPath, "DirectX", "Filtering");

					string aaUIOption = settingsForm.aaMapToUI[aaOption];
					string texfiltUIOption = settingsForm.texFiltMapToUI[texfiltOption];

					SetAATexFiltComboBoxes(aaUIOption, texfiltUIOption);
				}
				else // old wrapper or unknown wrapper
				{
					SetDGVControlState("Checked", false);
					SetDGVControlState("Enabled", false);

					settingsForm.DGVRadioButton.Checked = false;
					settingsForm.CRORadioButton.Checked = true;

					Settings.Default.Wrapper = "CRO";

					SetAATexFiltComboBoxes("Native", "Native");
				}
			}
			else // wrapper option disabled
			{
				SetDGVControlState("Checked", false);
				SetDGVControlState("Enabled", false);

				settingsForm.DGVRadioButton.Checked = false;
				settingsForm.CRORadioButton.Checked = false;
				settingsForm.DGVRadioButton.Enabled = false;
				settingsForm.CRORadioButton.Enabled = false;

				SetAATexFiltComboBoxes("Native", "Native");
			}
		}

		private void SetAATexFiltComboBoxes(string aaUIOption, string texfiltUIOption)
		{
			int aaIndex = settingsForm.AAComboBox.FindStringExact(aaUIOption);
			int texfiltIndex = settingsForm.TexFiltComboBox.FindStringExact(texfiltUIOption);

			if (aaIndex != -1) settingsForm.AAComboBox.SelectedIndex = aaIndex;
			if (texfiltIndex != -1) settingsForm.TexFiltComboBox.SelectedIndex = texfiltIndex;
		}

		private void SetDGVControlState(string mode, bool state)
		{
			if (mode == "Enabled")
			{
				settingsForm.AAComboBox.Enabled = state;
				settingsForm.TexFiltComboBox.Enabled = state;
				settingsForm.VSyncCheckBox.Enabled = state;
				settingsForm.FakeFullscreenAttrCheckBox.Enabled = state;
				settingsForm.StretchedARScalingCheckBox.Enabled = state;
				settingsForm.CaptureMouseCheckBox.Enabled = state;
				settingsForm.ToggleScreenModeCheckBox.Enabled = state;
				settingsForm.DefaultWindowedCheckBox.Enabled = state;
			}
			else if (mode == "Checked")
			{
				settingsForm.VSyncCheckBox.Checked = state;
				settingsForm.FakeFullscreenAttrCheckBox.Checked = state;
				settingsForm.StretchedARScalingCheckBox.Checked = state;
				settingsForm.CaptureMouseCheckBox.Checked = state;
				settingsForm.ToggleScreenModeCheckBox.Checked = state;
				settingsForm.DefaultWindowedCheckBox.Checked = state;
			}
		}

		public void HandleRegistryTasks()
		{
			WDUtils.CreateRegistryIfNotExist(regPath, "Language", "English");
			WDUtils.CreateRegistryIfNotExist(regPath + @"\Option", "lang", "0");

			string regValue_lang = Registry.GetValue(regPath + @"\Option", "lang", "0").ToString();
			string regValue_Language = Registry.GetValue(regPath, "Language", "English").ToString();

			//  if reg mismatched
			if ((regValue_lang == "0" && regValue_Language == "한국어") || (regValue_lang == "1" && regValue_Language == "English"))
			{
				// regValue_Language takes priority due to being hanled by the official launcher
				switch (regValue_Language)
				{
					case ("English"):
						Registry.SetValue(regPath + @"\Option", "lang", "0");
						regValue_lang = "0";
						break;
					case ("한국어"):
						Registry.SetValue(regPath + @"\Option", "lang", "1");
						regValue_lang = "1";
						break;
				}
			}

			Settings.Default.Save();

			// Set language option state
			bool KR = false;
			if (regValue_lang == "1") KR = true;
			LangRadioButton_EN.Checked = !KR;
			LangRadioButton_KR.Checked = KR;

			// Initialise text
			SetText(KR);
			settingsForm.SetText(KR);
		}

		public void SetText(bool KR)
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(KR ? "ko-KR" : "en");

			LaunchButtonImage = WDUtils.ResizeImage(Resources.whitedayexe, LaunchButton.Width);
			LaunchButtonHoverImage = WDUtils.ResizeImage(Resources.whitedayexe_hover, LaunchButton.Width);
			OhJaemiLaunchButtonImage = WDUtils.ResizeImage(Resources.ohjaemi, OhJaemiLaunchButton.Width);
			OhJaemiLaunchButtonHoverImage = WDUtils.ResizeImage(Resources.ohjaemi_hover, OhJaemiLaunchButton.Width);

			LaunchButton.Image = LaunchButtonImage;
			OhJaemiLaunchButton.Image = OhJaemiLaunchButtonImage;

			AdminModeLabel.Text = Resources.AdminModeTerm;
			AutoLaunchCheckBox.Text = Resources.AutoLaunchTerm;
			FixLocaleCheckBox.Text = Resources.FixLocaleTerm;
			D3D8WrapperCheckBox.Text = Resources.WrapD3DTerm;
			LangRadioButton_EN.Text = Resources.EnglishTerm;
			MoreSettingsLabel.Text = Resources.MoreSettingsTerm;

			hints.SetToolTip(VersionLabel, Resources.VersionTip);
			hints.SetToolTip(AdminModeLabel, Resources.AdminModeTip);
			hints.SetToolTip(D3D8WrapperCheckBox, Resources.D3D8WrapperTip);
			hints.SetToolTip(AutoLaunchCheckBox, Resources.AutoLaunchTip);
			hints.SetToolTip(FixLocaleCheckBox, Resources.LocaleEmulatorTip);
			hints.SetToolTip(OpenMainDirButton, Resources.OpenMainDirTip);
			hints.SetToolTip(LangRadioButton_EN, Resources.LangButtonTip);
			hints.SetToolTip(LangRadioButton_KR, Resources.LangButtonTip);
			hints.SetToolTip(OpenSavesDirButton, Resources.OpenSavesDirTip);
			hints.SetToolTip(OpenCaptureDirButton, Resources.OpenCaptureDirTip);
		}

		public void GetVersionFromRegistry()
		{
			WDUtils.CreateRegistryIfNotExist(regPath, "engversion", "");
			string regValue_engversion = Registry.GetValue(regPath, "engversion", "").ToString();
			VersionLabel.Text = regValue_engversion;
		}

		public Point CalculateSettingsFormLocation()
		{
			int x = this.Location.X + (this.Width - settingsForm.Width) / 2;
			int y = this.Location.Y + this.Height;
			return new Point(x, y);
		}

		// Buttons
		private void OpenMainDirButton_Click(object sender, EventArgs e)
		{
			Process.Start(DIR);
		}

		private void OpenSavesDirButton_Click(object sender, EventArgs e)
		{
			WDUtils.OpenVirtualDir("\\save");
		}

		private void OpenCaptureDirButton_Click(object sender, EventArgs e)
		{
			WDUtils.OpenVirtualDir("\\capture");
		}

		private void LaunchButton_Click(object sender, EventArgs e)
		{
			WDLaunchHandler.Start(this, settingsForm, "whiteday.exe", FixLocaleCheckBox.Checked, AutoLaunchCheckBox.Checked, "whiteday");
		}

		private void OhJaemiLaunchButton_Click(object sender, EventArgs e)
		{
			WDLaunchHandler.Start(this, settingsForm, "whiteday.exe", FixLocaleCheckBox.Checked, AutoLaunchCheckBox.Checked, "mod_beanbag");
		}

		private void ExitButton_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void VersionLabel_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.moddb.com/mods/white-day-repackaged/downloads");
		}

		private void AdminModeLabel_Click(object sender, EventArgs e)
		{
		}

		// Options
		private void D3D8WrapperCheckBox_Clicked(object sender, EventArgs e)
		{
			WDUtils.WDHelper("WRAPD3D", (!D3D8WrapperCheckBox.Checked).ToString(), Settings.Default.Wrapper);

			SetUIValues();
		}

		private void AutoLaunchCheckBox_Clicked(object sender, EventArgs e)
		{
			if (!IsDefaultDeviceKeyExists())
			{
				MessageBox.Show(Resources.AutoLaunchDeviceSettingsNotConfiguredWarning,
								Resources.WarningTerm,
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation);

				AutoLaunchCheckBox.Checked = false;
				Settings.Default.AutoLaunch = false;
				return;
			}

			Settings.Default.AutoLaunch = !Settings.Default.AutoLaunch;
			Settings.Default.Save();

			SetUIValues();
		}

		private bool IsDefaultDeviceKeyExists()
		{
			return Registry.GetValue(regPath, "DefaultDevice", null) != null;
		}

		private void FixLocaleCheckBox_Click(object sender, EventArgs e)
		{
			Settings.Default.UseLocaleEmulator = !Settings.Default.UseLocaleEmulator;
			Settings.Default.Save();

			SetUIValues();
		}

		private void LangRadioButton_EN_Click(object sender, EventArgs e)
		{
			SetLanguage("English", "LANG_EN", "1.18");
		}

		private void LangRadioButton_KR_Click(object sender, EventArgs e)
		{
			SetLanguage("한국어", "LANG_KR", "1.16");
		}

		private void SetLanguage(string targetLanguage, string langCode, string version)
		{
			if ((targetLanguage == "English" && LangRadioButton_EN.Checked) || (targetLanguage == "한국어" && LangRadioButton_KR.Checked))
				return;

			WDUtils.WDHelper(langCode);

			Registry.SetValue(regPath + @"\Option", "lang", targetLanguage == "English" ? "0" : "1");
			Registry.SetValue(regPath, "Language", targetLanguage);
			Registry.SetValue(regPath, "newversion", version);

			LangRadioButton_EN.Checked = targetLanguage == "English";
			LangRadioButton_KR.Checked = targetLanguage == "한국어";

			SetText(targetLanguage == "1");

			Application.Restart();
			Environment.Exit(0); // Ensures all foreground and background threads are terminated
		}

		// More settings
		private void MoreSettingsButton_Click(object sender, EventArgs e)
		{
			settingsForm.Visible = !settingsForm.Visible;
			settingsForm.Activate();
		}

		// Hover over button effects
		private readonly int[] LabelEnter = { 200, 200, 200 };
		private readonly int[] LabelLeave = { 255, 255, 255 };

		private void Button_MouseEnter(object sender, EventArgs e)
		{
			if (sender is PictureBox pb)
			{
				if (pb.Name == "LaunchButton")
				{
					pb.Image = LaunchButtonHoverImage;
				}
				else if (pb.Name == "ExitButton")
				{
					pb.Image = ExitButtonHoverImage;
				}
				else if (pb.Name == "OhJaemiLaunchButton")
				{
					pb.Image = OhJaemiLaunchButtonHoverImage;
					this.BackgroundImage = OJBgImage;
				}
			}
			else if (sender is Label lbl)
			{
				lbl.ForeColor = Color.FromArgb(LabelEnter[0], LabelEnter[1], LabelEnter[2]);
			}
		}

		private void Button_MouseLeave(object sender, EventArgs e)
		{
			if (sender is PictureBox pb)
			{
				if (pb.Name == "LaunchButton")
				{
					pb.Image = LaunchButtonImage;
				}
				else if (pb.Name == "ExitButton")
				{
					pb.Image = ExitButtonImage;
				}
				else if (pb.Name == "OhJaemiLaunchButton")
				{
					pb.Image = OhJaemiLaunchButtonImage;
					this.BackgroundImage = WDBgImage;
				}
			}
			else if (sender is Label lbl)
			{
				lbl.ForeColor = Color.FromArgb(LabelLeave[0], LabelLeave[1], LabelLeave[2]);
			}
		}

		// Moving the window
		private bool mouseDown;
		private Point lastLocation;

		private void WDLaunch_Form_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDown = true;
			lastLocation = e.Location;
		}

		private void WDLaunch_Form_MouseMove(object sender, MouseEventArgs e)
		{
			if (!mouseDown) return;

			this.Location = new Point(
				(this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

			this.Update();

			((WDLaunchSettings_Form)settingsForm).UpdateLocation(CalculateSettingsFormLocation());
		}

		private void WDLaunch_Form_MouseUp(object sender, MouseEventArgs e)
		{
			mouseDown = false;
		}
	}
}
