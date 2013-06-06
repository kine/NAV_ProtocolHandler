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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NVR_DynamicsNAVProtocolHandler
{
    /// <summary>
    /// Interaction logic for Toaster.xaml
    /// </summary>
    public partial class Toaster : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        public Toaster()
        {
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
            this.Topmost = true;
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
        }

        public Toaster(String text):this()
        {
            textBlock.Text = text;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
            //Application.Current.Shutdown();
        }
    }
}
