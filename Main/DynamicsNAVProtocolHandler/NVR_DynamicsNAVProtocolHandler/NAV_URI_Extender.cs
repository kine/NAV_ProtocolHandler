using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NVR_DynamicsNAVProtocolHandler
{
    /// <summary>
    /// Making "magic" with passed URI to find correct version of client and correct target NAV server.
    /// </summary>
    class NAV_URI_Extender
    {
        public static String GetVersionFromMapping(String URI)
        {
            var target = NAVClient2URI.GetByURI(URI);
            if (target != null)
            {
                return target.Version;
            }
            else
            {
                return "";
            }
        }

        public static Uri GetExtendedUri(Uri URI, uint pid)
        {
            if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.DisableMapping)
                return null;
            var navClients = new NAV_ROT.NAVClients();
            var navClient = navClients.GetClientByPID(pid);
            var target = NAVClient2URI.GetByServerDB(navClient.ServerName, navClient.DatabaseName, navClient.CompanyName);
            target.Company = navClient.CompanyName;
            if (target != null)
            {
                return ReparseUri(URI, target);
            }
            else
                return null;
        }

        /// <summary>
        /// Reparses the URI - add skipped parts like server name, instance name etc.
        /// </summary>
        /// <param name="URI">The URI.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private static Uri ReparseUri(Uri URI, Mapping target)
        {
            //dynamicsnav://///runpage?page=64005
            //dynamicsnav://server:7056/dynamicsnav70/CRONUS International Ltd./runpage?page=99000759
            if (URI.Authority!="") 
                return URI;
            var rest = URI.ToString().Substring(16);
            var result = URI.Scheme + "://" + target.NavServer +"/"+ target.Instance +"/" + target.Company + rest;
            return new Uri(result);
        }
    }
}
