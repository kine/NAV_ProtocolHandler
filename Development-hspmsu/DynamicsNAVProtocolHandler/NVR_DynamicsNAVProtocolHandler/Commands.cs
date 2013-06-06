using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NVR_DynamicsNAVProtocolHandler
{
    public static class Commands
    {
        private static readonly RoutedUICommand deleteMapping = new RoutedUICommand("Delete selected mapping", "DeleteMappingCommand", typeof(Commands));

        public static RoutedUICommand DeleteMappingCommand
        {
            get
            {
                return deleteMapping;
            }
        }

    }
}
