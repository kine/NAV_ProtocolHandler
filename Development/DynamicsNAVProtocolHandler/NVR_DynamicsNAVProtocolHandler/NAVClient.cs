using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NVR_DynamicsNAVProtocolHandler
{
    static class NAVClientFactory
    {
        /// <summary>
        /// Gets the NAV Client object based on passed version number.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Version not know: +version</exception>
        public static NAVClient GetObject(String version)
        {
            var parts = version.Split('.');
            switch (parts[0])
            {
                    //For version 6 and 7 do same process, use the 6 version class
                case @"6":
                case @"7": return new NAVClient6();
                default:
                    throw new ArgumentException("Version not known: "+version);
            }
        }
    }

    /// <summary>
    /// Interprets NAV client of different version which we will want to run and conenct to defined server.
    /// </summary>
    abstract class NAVClient
    {
        String path;
        String version;
        String server;
        String instance;
        String param;
        String uri;

        public String Uri
        {
            get { return uri; }
            set { 
                uri = value;
                var navUri = new NAV_URI(uri);
                Server = navUri.Server;
                Instance = navUri.Instance;
            }
        }

        public String Param
        {
            get { return param; }
            set { param = value; }
        }

        public String Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public String Server
        {
            get { return server; }
            set { server = value; }
        }

        public String Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets or sets the NAV client path, including exe file name.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public String Path
        {
            get { return path; }
            set {
                path = value;
                path = path.Replace(@" ""%1""", "");
                if (path.IndexOf("-protocolhandler") > 0)
                {
                    path = path.Replace(@" -protocolhandler", "");
                    param = @"-protocolhandler " + param;
                }
            }
        }

        public NAVClient()
        { }

        public NAVClient(String _version, String _uri, String _path)
        {
            Version = _version;
            Path = _path;
            Uri = _uri;
        }

        abstract public void PreRun();
        abstract public void PostRun();
        abstract public void RunClient(); 
    }

    /// <summary>
    /// Represents NAV client of version "6" (NAV 2009, 2009 SP1, 2009 R2)
    /// </summary>
    class NAVClient6 : NAVClient
    {
        /// <summary>
        /// Runs the client. It could be modified to create what is needed like some config files etc.
        /// All needed info should be available in the NAVClient base class properties like Server, Instance etc.
        /// Different versions of NAVClient could be used with different parameters and config files.
        /// </summary>
        public override void RunClient()
        {
            var procInfo = new ProcessStartInfo(Path, @"""" + Uri + @"""");
            procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Path);
            PreRun();
            Process.Start(procInfo);
            PostRun();
        }

        public override void PreRun()
        {
        }

        public override void PostRun()
        {
        }
    }
}
