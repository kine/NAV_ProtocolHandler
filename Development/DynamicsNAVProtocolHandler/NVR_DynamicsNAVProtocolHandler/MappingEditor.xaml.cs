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

namespace NVR_DynamicsNAVProtocolHandler
{
    /// <summary>
    /// Interaction logic for MappingEditor.xaml
    /// </summary>
    public partial class MappingEditor : Window
    {
        public MappingEditor()
        {
            InitializeComponent();
            NAVClient2URI.LoadMapping();
            //list.DataContext = NAVClient2URI.mappings;
            list.ItemsSource = NAVClient2URI.mappings;
        }

        public MappingEditor(Mapping mapping):this()
        {
            // TODO: Complete member initialization
            NAVClient2URI.mappings.Add(mapping);
            list.SelectedIndex = list.Items.Count-1;
            navServerTextBox.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NAVClient2URI.SaveMapping();
            this.DialogResult = true;
            this.Close();
        }
    }
}
