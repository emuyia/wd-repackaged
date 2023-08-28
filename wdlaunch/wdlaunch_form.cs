using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using WDLaunch.Properties;

namespace WDLaunch
{
	public partial class WDLaunch_Form : Form
	{
		WDLaunchSettings_Form settingsForm;
		private const string regPath = @"HKEY_CURRENT_USER\SOFTWARE\Sonnori\WhiteDay";
		private string Dir { get; set; }

		public WDLaunch_Form(string[] args)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(" + string.Join(", ", args.Select(a => $"\"{a}\"")) + ")");

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

			Dir = Directory.GetCurrentDirectory();

			settingsForm.aaMapToUI = settingsForm.aaMapToTech.ToDictionary(x => x.Value, x => x.Key);
			settingsForm.texFiltMapToUI = settingsForm.texFiltMapToTech.ToDictionary(x => x.Value, x => x.Key);

			if (WDUtils.CheckAdmin())
			{
				settingsForm.PrivilegeIndicatorLabel.ForeColor = Color.Red;
				settingsForm.PrivilegeIndicatorLabel.Text = "Admin mode";
			}
		}

		private void WDLaunch_Load(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

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

			//MoreSettingsButton.Visible = false;

			// Deprecated auto launch feature
			//if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			//{
			//	//WindowState = FormWindowState.Minimized;
			//	LaunchButton_Click(null, new EventArgs());
			//}
		}

		// Initialisation
		public void InitMouseOverButtons()
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

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
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			AutoLaunchCheckBox.Checked = Settings.Default.AutoLaunch;
			FixLocaleCheckBox.Checked = Settings.Default.UseLocaleEmulator;
			settingsForm.AlwaysAdminCheckBox.Checked = Settings.Default.AlwaysAdmin;
			D3D8WrapperCheckBox.Checked = !Convert.ToBoolean(WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "DirectX", "DisableAndPassThru"));

			AdminModeLabel.Visible = WDUtils.CheckAdmin();

			if (D3D8WrapperCheckBox.Checked)
			{
				settingsForm.AAComboBox.Enabled = true;
				settingsForm.TexFiltComboBox.Enabled = true;
				settingsForm.VSyncCheckBox.Enabled = true;
				settingsForm.FakeFullscreenAttrCheckBox.Enabled = true;
				settingsForm.StretchedARScalingCheckBox.Enabled = true;
				settingsForm.CaptureMouseCheckBox.Enabled = true;
				settingsForm.ToggleScreenModeCheckBox.Enabled = true;
				

				settingsForm.VSyncCheckBox.Checked = Convert.ToBoolean(
					WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "DirectX", "ForceVerticalSync"));

				settingsForm.FakeFullscreenAttrCheckBox.Checked = 
					WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "GeneralExt", "FullscreenAttributes") == "Fake";

				settingsForm.StretchedARScalingCheckBox.Checked = 
					WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "General", "ScalingMode") == "stretched_ar";

				settingsForm.CaptureMouseCheckBox.Checked = Convert.ToBoolean(
					WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "General", "CaptureMouse"));

				settingsForm.ToggleScreenModeCheckBox.Checked = !Convert.ToBoolean(
					WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "DirectX", "DisableAltEnterToToggleScreenMode"));

				if (settingsForm.ToggleScreenModeCheckBox.Checked)
				{
					settingsForm.DefaultWindowedCheckBox.Enabled = true;
					settingsForm.DefaultWindowedCheckBox.Checked =
						!Convert.ToBoolean(WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "General", "FullScreenMode")) &&
						!Convert.ToBoolean(WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "DirectX", "AppControlledScreenMode")) &&
						!Convert.ToBoolean(WDUtils.ReadDGVConfig($"{Dir}\\dgVoodoo.conf", "DirectX", "DisableAltEnterToToggleScreenMode"));
				}
				else
				{
					settingsForm.DefaultWindowedCheckBox.Enabled = false;
					settingsForm.DefaultWindowedCheckBox.Checked = false;
				}

				string aaOption = WDUtils.ReadDGVConfig(
					WDUtils.DGVConfPath, "DirectX", "Antialiasing");

				string texfiltOption = WDUtils.ReadDGVConfig(
					WDUtils.DGVConfPath, "DirectX", "Filtering");

				string aaUIOption = settingsForm.aaMapToUI[aaOption];
				string texfiltUIOption = settingsForm.texFiltMapToUI[texfiltOption];

				int aaIndex = settingsForm.AAComboBox.FindStringExact(aaUIOption);
				int texfiltIndex = settingsForm.TexFiltComboBox.FindStringExact(texfiltUIOption);

				if (aaIndex != -1) settingsForm.AAComboBox.SelectedIndex = aaIndex;
				if (texfiltIndex != -1) settingsForm.TexFiltComboBox.SelectedIndex = texfiltIndex;
			}
			else
			{
				int aaIndex = settingsForm.AAComboBox.FindStringExact("Native");
				int texfiltIndex = settingsForm.TexFiltComboBox.FindStringExact("Native");

				if (aaIndex != -1) settingsForm.AAComboBox.SelectedIndex = aaIndex;
				if (texfiltIndex != -1) settingsForm.TexFiltComboBox.SelectedIndex = texfiltIndex;

				settingsForm.VSyncCheckBox.Checked = false;
				settingsForm.FakeFullscreenAttrCheckBox.Checked = false;
				settingsForm.StretchedARScalingCheckBox.Checked = false;
				settingsForm.CaptureMouseCheckBox.Checked = false;
				settingsForm.ToggleScreenModeCheckBox.Checked = false;
				settingsForm.DefaultWindowedCheckBox.Checked = false;

				settingsForm.AAComboBox.Enabled = false;
				settingsForm.TexFiltComboBox.Enabled = false;
				settingsForm.VSyncCheckBox.Enabled = false;
				settingsForm.FakeFullscreenAttrCheckBox.Enabled = false;
				settingsForm.StretchedARScalingCheckBox.Enabled = false;
				settingsForm.CaptureMouseCheckBox.Enabled = false;
				settingsForm.ToggleScreenModeCheckBox.Enabled = false;
				settingsForm.DefaultWindowedCheckBox.Enabled = false;
			}
		}

		public void HandleRegistryTasks()
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

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

			// Initialise this program's text
			SetText(KR);
		}

		public void SetText(bool KR)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}(KR = {KR})");

			Thread.CurrentThread.CurrentUICulture = new CultureInfo(KR ? "ko-KR" : "en");

			AdminModeLabel.Text = Resources.AdminModeTerm;
			settingsForm.AAComboBoxLabel.Text = Resources.AATerm;
			settingsForm.TexFiltComboBoxLabel.Text = Resources.TexFiltTerm;

			AutoLaunchCheckBox.Text = Resources.AutoLaunchTerm;
			FixLocaleCheckBox.Text = Resources.FixLocaleTerm;
			D3D8WrapperCheckBox.Text = Resources.WrapD3DTerm;

			LangRadioButton_EN.Text = Resources.EnglishTerm;

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

			hints.SetToolTip(settingsForm.AAComboBox, Resources.AATip);
			hints.SetToolTip(settingsForm.TexFiltComboBox, Resources.TexFiltTip);
		}

		public void GetVersionFromRegistry()
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

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
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			Process.Start(Dir);
		}

		private void OpenSavesDirButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			WDUtils.OpenVirtualDir("\\save");
		}

		private void OpenCaptureDirButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			WDUtils.OpenVirtualDir("\\capture");
		}

		private void LaunchButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			WDLaunchHandler.Start(this, settingsForm, "whiteday.exe", FixLocaleCheckBox.Checked, AutoLaunchCheckBox.Checked, "whiteday");
		}

		private void OhJaemiLaunchButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			WDLaunchHandler.Start(this, settingsForm, "whiteday.exe", FixLocaleCheckBox.Checked, true, "mod_beanbag");
		}

		private void ExitButton_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			Application.Exit();
		}

		private void VersionLabel_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			Process.Start("https://www.moddb.com/mods/white-day-repackaged/downloads");
		}

		private void AdminModeLabel_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");
		}

		// Options
		private void D3D8WrapperCheckBox_Clicked(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			WDUtils.WDHelper("WRAPD3D", (!D3D8WrapperCheckBox.Checked).ToString());

			SetUIValues();
		}

		private void AutoLaunchCheckBox_Clicked(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			Settings.Default.AutoLaunch = !Settings.Default.AutoLaunch;
			Settings.Default.Save();

			SetUIValues();
		}

		private void FixLocaleCheckBox_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			Settings.Default.UseLocaleEmulator = !Settings.Default.UseLocaleEmulator;
			Settings.Default.Save();

			SetUIValues();
		}

		private void LangRadioButton_EN_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			SetLanguage("English", "LANG_EN", "1.18");
		}

		private void LangRadioButton_KR_Click(object sender, EventArgs e)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}()");

			SetLanguage("한국어", "LANG_KR", "1.16");
		}

		private void SetLanguage(string targetLanguage, string langCode, string version)
		{
			Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}({targetLanguage}, {langCode}, {version})");

			Console.WriteLine($"targetLanguage = {targetLanguage}");
			Console.WriteLine($"LangRadioButton_EN.Checked = {LangRadioButton_EN.Checked}");
			Console.WriteLine($"LangRadioButton_KR.Checked = {LangRadioButton_KR.Checked}");

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
					pb.Image = Resources.whitedayexe_hover;
				}
				else if (pb.Name == "ExitButton")
				{
					pb.Image = Resources._140_hover;
				}
				else if (pb.Name == "OhJaemiLaunchButton")
				{
					pb.Image = Resources.ohjaemi_hover;
					this.BackgroundImage = Resources.ojbg;
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
					pb.Image = Resources.whitedayexe;
				}
				else if (pb.Name == "ExitButton")
				{
					pb.Image = Resources._140;
				}
				else if (pb.Name == "OhJaemiLaunchButton")
				{
					pb.Image = Resources.ohjaemi;
					this.BackgroundImage = Resources.wdbg;
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
