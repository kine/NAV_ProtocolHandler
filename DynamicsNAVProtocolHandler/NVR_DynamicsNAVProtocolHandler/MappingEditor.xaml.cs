using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            list.Items.SortDescriptions.Add(new SortDescription("DbServer", ListSortDirection.Ascending));
            list.Items.SortDescriptions.Add(new SortDescription("Db", ListSortDirection.Ascending));
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

        private void DeleteMappingCommand_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            NAVClient2URI.mappings.Remove((Mapping)list.SelectedItem);
        }

        private void DeleteMappingCommand_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute=list.SelectedIndex >= 0;
            e.Handled = true;
        }
    }
}
