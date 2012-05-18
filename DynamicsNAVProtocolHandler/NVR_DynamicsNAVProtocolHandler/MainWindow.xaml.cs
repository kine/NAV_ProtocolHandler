using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Security;

namespace NVR_DynamicsNAVProtocolHandler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (NVR_DynamicsNAVProtocolHandler.App.haveParams)
            {
                this.Hide();
            }
            CheckIfActive();
            versionLabel.Content = "ver "+System.Diagnostics.FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).FileVersion;
        }

        private void CheckIfActive()
        {
            string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "XXX");
            var isActive = (defaultPath != "XXX") && (defaultPath != "");
            button.Content = (isActive ? "Deactivate" : "Activate");
        }

        private void ChangeActive()
        {
            string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "XXX");
            var isActive = (defaultPath != "XXX") && (defaultPath != "");
            if (isActive)
            {
                DeactivateHandler();
            }
            else
            {
                ActivateHandler();
            }
            CheckIfActive();
        }

        private void ActivateHandler()
        {
            try
            {
                string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "", "");
                Microsoft.Win32.Registry.SetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", defaultPath, Microsoft.Win32.RegistryValueKind.String);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "", Process.GetCurrentProcess().MainModule.FileName + @" ""%1""");
            }
            catch (SecurityException e)
            {
                MessageBox.Show("Security Error", "You do not have enough permissions to modify the registry.\nTry to run the app as Administrator");
            }
        }

        private void DeactivateHandler()
        {
            try {
                string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
                Microsoft.Win32.Registry.SetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "", defaultPath);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
            }
            catch (SecurityException e)
            {
                MessageBox.Show("Security Error", "You do not have enough permissions to modify the registry.\nTry to run the app as Administrator");
            }
        }

        private void buttonClose_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            ChangeActive();
        }
    }
}
