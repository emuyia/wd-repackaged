using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDLaunch.Properties;

namespace WDLaunch
{
	public partial class WDLaunchSettings_Form : Form
	{
		private WDLaunch_Form mainForm;

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

		private void DGVRadioButton_Click(object sender, EventArgs e)
		{
			string D3D8Author = FileVersionInfo.GetVersionInfo(Path.Combine(WDUtils.Dir, "d3d8.dll")).CompanyName;

			if (D3D8Author != "Dégé")
			{
				WDUtils.WDHelper("D3DDGV");
			}

			mainForm.SetUIValues();
		}

		private void CRORadioButton_Click(object sender, EventArgs e)
		{
			string D3D8Author = FileVersionInfo.GetVersionInfo(Path.Combine(WDUtils.Dir, "d3d8.dll")).CompanyName;

			if (D3D8Author != "Dégé")
			{
				WDUtils.WDHelper("D3DCRO");
			}

			mainForm.SetUIValues();
		}
	}
}
