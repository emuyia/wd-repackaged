using System;
using System.Windows.Forms;

namespace WDLaunch
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            WDLaunch_Form WDLForm = new WDLaunch_Form(args);
            Application.Run(WDLForm);
        }
    }
}