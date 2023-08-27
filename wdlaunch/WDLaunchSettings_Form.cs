using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDLaunch
{
	public partial class WDLaunchSettings_Form : Form
	{
		public WDLaunchSettings_Form()
		{
			InitializeComponent();

			this.BackColor = Color.Magenta;
			this.TransparencyKey = Color.Magenta;
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
	}
}
