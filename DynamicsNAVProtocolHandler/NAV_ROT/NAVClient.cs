using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Globalization;
using System.Collections;
using Win32Helper;

namespace NAV_ROT
{
    /// <summary>
    /// Class representing one runing client
    /// </summary>
    public class NAVClient : IDisposable
    {

        #region private fields
        private Guid myId = Guid.NewGuid();
        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private object rotObject;
        private INSAppBase appBase;
        private INSApplication application;
        private INSHyperlink hyperlink;
        private INSTable table;
        private INSRec rec;
        private IntPtr hwnd;
        private String serverName;
        private String databaseName;
        private String companyName;
        private String userName;
        private uint processID;
        private IConnectionPoint CP = null;
        private int cookie;
        private String id;
        private String applicationVersion;
        private String csideVersion;
        private int serverType;

        #endregion

        #region Public Properties
        public Guid MyId
        {
            get { return myId; }
        }
        public String ApplicationVersion
        {
            get { return applicationVersion; }
        }

        public String CSIDEVersion
        {
            get { return csideVersion; }
        }

        public int ServerType
        {
            get { return serverType; }
        }

        public String Id
        {
            get { return id; }
        }

        public object RotObject
        {
            get { return rotObject; }
        }

        public INSAppBase AppBase
        {
            get { return appBase; }
        }

        public INSApplication Application
        {
            get { return application; }
        }

        public INSHyperlink Hyperlink
        {
            get { return hyperlink; }
        }

        public INSTable Table
        {
            get { return table; }
            set { table = value; }
        }

        public INSRec Rec
        {
            get { return rec; }
            set { rec = value; }
        }

        public IntPtr Hwnd
        {
            get { return hwnd; }
        }

        public String ServerName
        {
            get { return serverName; }
        }


        public String DatabaseName
        {
            get { return databaseName; }
        }

        public String CompanyName
        {
            get { return companyName; }
        }

        public String UserName
        {
            get { return userName; }
        }

        public uint ProcessID
        {
            get { return processID; }
        }

        public String InstanceName
        {
            get { return serverName + ";" + databaseName + ";" + companyName + ";" + userName; }
        }

        #endregion


        public NAVClient()
        {
        }


        private System.Threading.Timer timerCheckWindows = null;
        Hashtable designWindows = new Hashtable();


        public NAVClient(object navObject, object clientID)
            : this(navObject)
        {
            id = (String)clientID;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NAVClient"/> class.
        /// </summary>
        /// <param name="navObject">The nav object from ROT.</param>
        public NAVClient(object navObject)
        {
            Debug.WriteLine("NAVClient.New:" + myId);
            watchers = new List<FileSystemWatcher>();
            rotObject = navObject;
            appBase = (INSAppBase)navObject;
            application = navObject as INSApplication;
            hyperlink = navObject as INSHyperlink;
            if (appBase == null)
            {
                throw (new InvalidDataException(@"ROT Object is not NAV!"));
            }
            if (hyperlink != null)
            {
                do
                {
                    int handle;
                    try
                    {
                        hyperlink.GetNavWindowHandle(out handle);
                        hwnd = new IntPtr(handle);
                        uint res = Win32.User32.GetWindowThreadProcessId(hwnd, out processID);
                        StringBuilder text = new StringBuilder(1000);
                        int res2 = Win32.User32.GetWindowText(hwnd, text, 1000);
                        //if (text.ToString().IndexOf("TFS TRACKED") == -1)
                        //    Win32.User32.SetWindowText(hwnd, "TFS TRACKED " + text);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(500);
                    }
                } while (hwnd == IntPtr.Zero);
            }

            appBase.GetInfos(out serverName, out databaseName, out companyName, out userName);
        }

        #region IDisposable Members
        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine("NAVClient.Dispose(" + disposing.ToString() + ") " + this.myId);
            if (disposing) //managed resources release
            {
                if (watchers != null)
                {
                    foreach (FileSystemWatcher watcher in watchers)
                    {
                        watcher.Dispose();
                    }
                    watchers = null;
                }
            }
            try
            {
                if ((cookie != 0) && (CP != null))
                {
                    CP.Unadvise(cookie);
                    cookie = 0;
                }
                if (timerCheckWindows != null)
                {
                    timerCheckWindows.Dispose();
                    timerCheckWindows = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Release:" + e.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
