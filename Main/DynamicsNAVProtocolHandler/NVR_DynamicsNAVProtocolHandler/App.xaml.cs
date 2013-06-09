using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Win32Helper;
using System.Diagnostics;
using System.IO;

namespace NVR_DynamicsNAVProtocolHandler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool haveParams = false;
        public static string args = "";
        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            try
            {
                NAVClient2URI.LoadMapping();
                args = "";
                foreach (String a in e.Args)
                {
                    if (args != "")
                        args += " ";
                    args += a;
                }
                if (args == "")
                    return;

                haveParams = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
