using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Linq;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Text.RegularExpressions;
using System.Data;

namespace NAV_ROT
{

    public class ClientID
    {
        private String id;

        public String Id
        {
            get { return id; }
        }
        public ClientID(object obj)
        {
            id = obj.GetHashCode().ToString();
        }
    }
    /// <summary>
    /// Class representing all running NAV Clients
    /// </summary>
    public class NAVClients : IDisposable
    {
        internal List<NAVClient> clients;
        internal System.Threading.Timer processWatcher;

        private NAVClient lastActiveClient;

        /// <summary>
        /// Gets the last active client, based on OnClientActive events from clients.
        /// </summary>
        /// <value>The last active client.</value>
        public NAVClient LastActiveClient
        {
            get { return lastActiveClient; }
        }

        /// <summary>
        /// List of all last read objects from ROT with their NAVClient representation
        /// </summary>
        private Hashtable objects;

        /// <summary>
        /// Gets the running clients.
        /// </summary>
        /// <value>The clients collection.</value>
        public List<NAVClient> Clients
        {
            get { return clients; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAVClients"/> class.
        /// During initialization the ROT is iterated and all NAV clients are stored and connected to the events.
        /// </summary>
        public NAVClients()
        {
            clients = new List<NAVClient>();
            objects = new Hashtable();
            Refresh();
        }

        /// <summary>
        /// Adds the clients into collection if not already there.
        /// </summary>
        /// <param name="objName">The NAV client obj. from ROT</param>
        /// <returns>New or existing NAVClient object representing given obj</returns>
        private NAVClient AddClient(object clientID, object clientObj)
        {
            /*
            foreach (object obj in objects.Keys)
            {
                if (((NAVClient)objects[obj]).RotObject.Equals(clientObj))
                {
                    return (NAVClient)objects[obj];
                }
            }
             * */

            NAVClient client=null;
            try
            {
                client = new NAVClient(clientObj, clientID);
                if (objects.ContainsKey(clientID))
                {
                    client.Dispose();
                    return (NAVClient)objects[clientID];
                }
                objects.Add(clientID, client);
                clients.Add(client);
                Debug.WriteLine("Adding:" + clientID+" "+client.MyId);
                return client;
            }
            catch (Exception)
            {
                if (client != null)
                    client.Dispose();
                return null;
            }
        }

        /// <summary>
        /// Removes the client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void RemoveClient(NAVClient client)
        {
            Debug.WriteLine("Removing:" + client.Hwnd);
            if (lastActiveClient == client)
            {
                lastActiveClient = null;
            }
            clients.Remove(client);
            if (objects[client.Hwnd] != null)
                Marshal.ReleaseComObject(client.RotObject);
            objects.Remove(client.Hwnd);
            client.Dispose();
        }

        /// <summary>
        /// Removes the client by pid.
        /// </summary>
        /// <param name="pid">The Process ID.</param>
        public void RemoveClientByPid(int pid)
        {
            foreach (NAVClient client in clients)
            {
                if (client.ProcessID == pid)
                {
                    RemoveClient(client);
                    return;
                }
            }
        }

        private object lockthis = new object();
        /// <summary>
        /// Refreshes list of running clients.
        /// </summary>
        public void Refresh()
        {
            lock (lockthis)
            {
                Debug.WriteLine("Refreshing clients...");
                Hashtable rot = ROT.GetROT(); //Read all objects of type C/SIDE from ROT
                Hashtable newClients = new Hashtable();

                foreach (object obj in rot.Keys)
                {
                    //ClientID clientID=new ClientID(rot.ObjectList[obj]);
                    //newClients.Add(clientID, AddClient(clientID, rot.ObjectList[obj]));
                    NAVClient client = null;
                    try {
                        client = new NAVClient(rot[obj]);
                        if (!objects.ContainsKey(client.Hwnd))
                        {
                            objects[client.Hwnd] = client;
                            //client.WatchObjChanges();
                            clients.Add(client);
                            newClients[client.Hwnd] = client;
                            Debug.WriteLine("Adding:" + client.Hwnd.ToString() + " " + client.MyId+" "+client.InstanceName);
                        }
                        else
                        {
                            newClients[client.Hwnd] = objects[client.Hwnd];
                            client.Dispose();
                        }
                    } catch (Exception) 
                    {
                        if (client != null)
                            client.Dispose();
                    }
                }

                /// Remove deleted objects
                List<object> clientToDelete = new List<object>();
                foreach (object clientID in objects.Keys)
                {
                    if (!newClients.ContainsKey(clientID))
                    {
                        clientToDelete.Add(clientID);
                    }
                }

                foreach (object clientID in clientToDelete)
                {
                    RemoveClient((NAVClient)objects[clientID]);
                }
            }
        }

        public NAVClient GetClientByPID(uint pid)
        {
            IEnumerable<NAVClient> clientsWithPid = from c in clients where c.ProcessID==pid select c;
            if (clientsWithPid!=null)
              return clientsWithPid.First();
            return null;
        }

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (processWatcher != null)
                {
                    processWatcher.Dispose();
                    processWatcher = null;
                }
            }
            foreach (NAVClient c in clients)
            {
                c.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class ROT
    {
        public static void CheckInterface(object obj)
        {
            IObjectDesigner designer = obj as IObjectDesigner;
            Debug.WriteLineIf(designer != null, "Is IObjectDesigner");//*
            INSCallbackEnum callbackEnum = obj as INSCallbackEnum;
            Debug.WriteLineIf(callbackEnum != null, "Is INSCallbackEnum");
            INSRec rec = obj as INSRec;
            Debug.WriteLineIf(rec != null, "Is INSRec");
            INSAppBase appBase = obj as INSAppBase;
            Debug.WriteLineIf(appBase != null, "Is INSAppBase");//*
            INSApplication app = obj as INSApplication;
            Debug.WriteLineIf(app != null, "Is INSApplication");//*
            INSForm form = obj as INSForm;
            Debug.WriteLineIf(form != null, "Is INSForm");
            INSHook hook = obj as INSHook;
            Debug.WriteLineIf(hook != null, "Is INSHook");
            INSHyperlink hyper = obj as INSHyperlink;
            Debug.WriteLineIf(hyper != null, "Is INSHyperlink");//*
            INSMenuButton menub = obj as INSMenuButton;
            Debug.WriteLineIf(menub != null, "Is INSMenuButton");
            INSTable table = obj as INSTable;
            Debug.WriteLineIf(table != null, "Is INSTable");
            IConnectionPointContainer conn = obj as IConnectionPointContainer;
            Debug.WriteLineIf(conn != null, "Is IConnectionPointContainer");
        }
        public static object lockthis = new object();

        public static Hashtable GetROT()
        {
            lock (lockthis)
            {
                Hashtable objectList;
                List<IMoniker> rotList;
                rotList = new List<IMoniker>();
                objectList = new Hashtable();
                IBindCtx ctx = null;
                IRunningObjectTable table = null;
                IEnumMoniker mon = null;
                IMoniker[] lst = new IMoniker[1];
                try
                {
                    NativeMethods.CreateBindCtx(0, out ctx);
                    ctx.GetRunningObjectTable(out table);
                    table.EnumRunning(out mon);
                    Debug.WriteLine("ROT:");
                    Debug.Indent();
                    while (mon.Next(1, lst, IntPtr.Zero) == 0)
                    {
                        string displayName;
                        lst[0].GetDisplayName(ctx, lst[0], out displayName);
                        //Debug.WriteLine(displayName);
                        if ((displayName.IndexOf("!C/SIDE!") != -1) /*&& (displayName.IndexOf("database=") !=-1)*/)
                        {
                            try
                            {
                                rotList.Add(lst[0]);
                                object obj;
                                int ret=table.GetObject(lst[0], out obj);
                                ClientID clientID = new ClientID(obj);

                                int hash;
                                lst[0].Hash(out hash);
                                using (NAVClient c = new NAVClient(obj)) {
                                    //CheckInterface(obj);
                                    Debug.WriteLine(c.Hwnd + ":" + displayName);
                                    objectList[clientID.Id] = obj;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("ROT NAV Obj exception:" + e.Message);
                            }
                        }
                    }
                    Debug.Unindent();
                    return objectList;
                }
                finally
                {
                    if (ctx != null)
                        Marshal.ReleaseComObject(ctx);
                    if (mon != null)
                        Marshal.ReleaseComObject(mon);
                    if (table != null)
                        Marshal.ReleaseComObject(table);
                }
            }
        }
    }

    public enum NavObjectType
    {
        None = 0,
        Page = 8,
        MenuSuite = 7,
        XMLPort = 6,
        Codeunit = 5,
        Dataport = 4,
        Form = 2,
        Report = 3,
        Table = 1
    }


    /// <summary>
    /// class representing NAV object
    /// </summary>
    public class NAVObject
    {
        NavObjectType type;
        String versionList;
        DateTime date;
        DateTime time;
        bool modified;
        bool compiled;

        public bool Compiled
        {
            get { return compiled; }
            set { compiled = value; }
        }

        public bool Modified
        {
            get { return modified; }
            set { modified = value; }
        }
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public String VersionList
        {
            get { return versionList; }
            set { versionList = value; }
        }
        public NavObjectType Type
        {
            get { return type; }
            set { type = value; }
        }
        int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public NAVObject() { }

        public NAVObject(String line)
        {
            if (line.IndexOf("OBJECT") != 0) throw (new InvalidDataException("Not OBJECT line"));
            String pattern = "OBJECT (\\w+) (\\d+) (.+)";
            string[] parts = Regex.Split(line, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            id = Convert.ToInt32(parts[2]);
            name = parts[3];
            type = (NavObjectType)Enum.Parse(typeof(NavObjectType), parts[1], true);
        }

        public NAVObject(NavObjectType _type, int _id, String _name = "")
        {
            type = _type;
            id = _id;
            name = _name;
        }

    }
}
