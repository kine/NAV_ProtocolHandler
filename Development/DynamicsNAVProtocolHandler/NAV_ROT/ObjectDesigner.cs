using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using NAV_ROT;

namespace NAV_ROT
{
    [ComImport, SuppressUnmanagedCodeSecurity, Guid("1CF2B120-547D-101B-8E65-08002B2BD119"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IErrorInfo
    {
        [PreserveSig]
        int GetGUID();
        [PreserveSig]
        int GetSource([MarshalAs(UnmanagedType.BStr)] out string pBstrSource);
        [PreserveSig]
        int GetDescription([MarshalAs(UnmanagedType.BStr)] out string pBstrDescription);
    }

    public class NativeMethods
    {
        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetErrorInfo(int dwReserved, [MarshalAs(UnmanagedType.Interface)] out IErrorInfo ppIErrorInfo);

        [DllImport("ole32.dll", PreserveSig = false)]
        internal static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        [DllImport("ole32.dll", PreserveSig = false)]
        internal static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return"), DllImport("ole32.dll")]
        internal static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("OLE32.DLL")]
        internal static extern int CreateStreamOnHGlobal(int hGlobalMemHandle, bool fDeleteOnRelease, out IStream pOutStm);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return"), DllImport("ole32.dll")]
        internal static extern void GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        internal static extern int ProgIDFromCLSID([In()]ref Guid clsid, [MarshalAs(UnmanagedType.LPWStr)]out string lplpszProgID);

    }

    public class NAVForm
    {
        public NAVForm() { }
        private String id;

        public String ID
        {
            get { return id; }
            set { id = value; }
        }
        private String number;

        public String Number
        {
            get { return number; }
            set { number = value; }
        }
        private String link;

        public String Link
        {
            get { return link; }
            set { link = value; }
        }
        private String tableID;

        public String TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        private String recID;

        public String RecID
        {
            get { return recID; }
            set { recID = value; }
        }
    }

    public class NAVEnumTest
    {
        private object o;
        public NAVEnumTest(object _o) 
        {
            o = _o;
            //INSCallbackEnum
        }

    }
    /****************************/
    internal class CallBack : INSCallbackEnum
    {
        #region INSCallbackEnum Members
        public INSRec lastRec;
        public int NextRecord(INSRec record)
        {
            int a=1;
            String b;
            record.GetFieldValue(a,out b);
            Debug.WriteLine("NextRecord=" + b);
            CallBack callbackTab = new CallBack();
            Debug.Indent();
            record.EnumFieldValues(callbackTab);
            Debug.Unindent();
            lastRec = record;
            return 1;
        }

        public int NextFieldValue(int fieldID, string fieldValue, string dataType)
        {
            Debug.WriteLine("NextFieldValue=" + fieldID.ToString()+";"+fieldValue+";"+dataType);
            return 1;
        }

        public int NextFilterValue(int fieldNo, string filterValue)
        {
            Debug.WriteLine("NextFilterValue=" + fieldNo.ToString() + ";" + filterValue);
            return 1;
        }

        public int NextTable(int tableNo, string tableName)
        {
            Debug.WriteLine("NextTable=" + tableNo.ToString() + ";" + tableName);
            return 1;
        }

        public int NextFieldDef(int fieldID, string fieldName, string fieldCaption, string dataType, int dataLength, int f)
        {
            Debug.WriteLine("NextFieldDef=" + fieldID.ToString() + ";" + fieldName+";"+fieldCaption+";"+dataType+";"+dataLength+";f="+f.ToString());
            return 1;
        }

        #endregion
    }
    internal class AppEvents : INSApplicationEvents 
    {
        #region INSApplicationEvents Members

        public int OnFormOpen(INSForm form)
        {
            String id;
            form.GetID(out id);
            Debug.WriteLine("AppEvent.OnFormOpen:" + id.ToString());
            return 0;
        }

        public int proc4(INSForm form, string b)
        {
            String id;
            form.GetID(out id);
            Debug.WriteLine("AppEvent.Proc4:" + id.ToString()+" "+b);
            return 1;
        }

        public int OnActiveChanged(bool a)
        {
            Debug.WriteLine("AppEvent.OnActiveChanged:" + a.ToString());
            return 0;
        }

        public int OnCompanyClose()
        {
            Debug.WriteLine("AppEvent.CompanyClose");
            return 1;
        }

        #endregion
    }
    internal class NAVHook : INSHook
    {
        #region INSHook Members

        public int proc3(INSAppBase appBase)
        {
            String server,db,company,user;
            appBase.GetInfos(out server,out db, out company, out user);
            Debug.WriteLine("Hook.Proc3:" + server + " " + db + " " + company + " " + user);
            return 0;
        }

        #endregion
    }
    internal class NAVButtonEvent : INSMenuButtonEvents
    {
        #region INSMenuButtonEvents Members

        public int proc3()
        {
            Debug.WriteLine("Button.Proc3");
            return 0;
        }

        public int proc4(int a)
        {
            Debug.WriteLine("Button.Proc4: a="+a.ToString());
            return 0;
        }

        #endregion
    }
    public class ObjectDesignerTest
    {
        private IObjectDesigner _objectDesigner;
        private const string DefaultMonikerName = "!C/SIDE";
        private int codepage;
        //private const string ObjectDesignerGuid = "50000004-0000-1000-0001-0000836BD2D2";

        public ObjectDesignerTest()
        {
            
            Hashtable runningObjects = GetActiveObjectList(DefaultMonikerName);
            foreach (DictionaryEntry de in runningObjects)
            {
                string progId = de.Key.ToString();
                if (progId.IndexOf("{") != -1)
                {
                    // Convert a class id into a friendly prog Id
                    progId = ConvertClassIdToProgId(de.Key.ToString());
                }
                object getObj = GetActiveObject(progId);
                
                if (getObj != null)
                {
                    INSApplication app = getObj as INSApplication;
                    IObjectDesigner objDis = getObj as IObjectDesigner;
                    INSAppBase appBase = getObj as INSAppBase;
                   
                    INSHyperlink hyp = getObj as INSHyperlink;
                    //Debug.WriteLine("App.="+app.GetType().FullName);
                    if (hyp != null)
                    {
                        int handle;
                        hyp.GetNavWindowHandle(out handle);
                        Debug.WriteLine("WindowHandle=" + handle.ToString());
                    }
                    if (appBase != null)
                    {
                        int res=0;
                        
                        try
                        {
                            //res = appBase.Error("What<???");
                        } catch (Exception) {}
                        
                        //appBase.proc6();
                        //appBase.GetInfos(out a, out b,out c, out d);
                        Debug.WriteLine("Appbase=");
                        Debug.Indent();
                        Debug.WriteLine("res=" + res);
                        //Debug.WriteLine("b=" + b);
                        //Debug.WriteLine("c=" + c);
                        //Debug.WriteLine("d=" + d);
                        Debug.Unindent();
                    }
                    if (objDis != null)
                    {
                        String version;
                        int res=objDis.GetDatabaseName(out version);
                        Debug.WriteLine("ObjDis=" + version);
                    }
                    if (app != null)
                    {
                        INSForm form;
                        Debug.WriteLine("appGetform=" + app.GetForm(out form).ToString());
                        String hyperlink;
                        if (form != null)
                        {
                            form.GetID(out hyperlink);
                            /*form.GetButtons(out button);
                            if (button != null)
                            {
                                //button.proc3("Run");
                                //button.proc4(0,"MyButtonB","MyButtonC");
                                //Debug.WriteLine("Button="+button.
                            }
                             * */
                            INSRec rec;
                            CallBack callback=new CallBack();
                            form.GetRec(out rec);
                            rec.EnumFieldValues(callback);
                            Debug.WriteLine("form=" + form.ToString());
                            if (hyperlink != null)
                                Debug.WriteLine("FormID=" + hyperlink);
                        }
                    }
                    //this._objectDesigner = getObj as IObjectDesigner;
                }
                else
                {
                    //Console.WriteLine("!!!!!FAILED TO fetch: " + progId);
                }
            }
            /*
            if (this._objectDesigner == null)
            {
                throw new Exception("Could not connect to Dynamics NAV");
            }
             * */

        }

        public ObjectDesignerTest(String serverNam, String dbName, String companyName, int codePage)
        {
            Hashtable runningObjects = GetActiveObjectList(DefaultMonikerName);
            this.codepage = codePage;
            foreach (DictionaryEntry de in runningObjects)
            {
                string progId = de.Key.ToString();
                if (progId.IndexOf("{") != -1)
                {
                    // Convert a class id into a friendly prog Id
                    progId = ConvertClassIdToProgId(de.Key.ToString());
                }
                object getObj = GetActiveObject(progId);
                if (getObj != null)
                {
                    this._objectDesigner = getObj as IObjectDesigner;
                    if (this._objectDesigner == null)
                    {
                        throw new Exception("Could not connect to Dynamics NAV");
                    }
                    else
                    {
                        String company, server, db;
                        company = GetCompanyName();
                        db = GetDatabaseName();
                        server = GetServerName();
                        if ((company == companyName) && (db == dbName) && (server.ToUpper() == serverNam.ToUpper()))
                        {
                            break;
                        }
                        else
                        {
                            this._objectDesigner = null;
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("!!!!!FAILED TO fetch: " + progId);
                }
            }
            if (this._objectDesigner == null)
            {
                throw new Exception("Could not connect to Dynamics NAV");
            }

        }

        public void CompileObject(NavObjectType navObjectType, int objectId)
        {
            int result = this._objectDesigner.CompileObject((int)navObjectType, objectId);
            this.ProcessResult(result);
        }

        public void CompileObjects(string filter)
        {
            int result = this._objectDesigner.CompileObjects(filter);
            this.ProcessResult(result);
        }

        string ConvertString(string text)
        {
            Encoding defaultEnc = Encoding.Default;
            Encoding navEnc = Encoding.GetEncoding(this.codepage);

            // Convert the string into a byte[].
            byte[] asciiBytes = defaultEnc.GetBytes(text);

            // Perform the conversion from one encoding to the other.

            String result = navEnc.GetString(asciiBytes);
            return result;
        }

        public string GetCompanyName()
        {
            string companyName ="";
            int num = this._objectDesigner.GetCompanyName(out companyName);
            return ConvertString(companyName);
        }

        public string GetDatabaseName()
        {
            string databaseName;
            int num = this._objectDesigner.GetDatabaseName(out databaseName);
            return ConvertString(databaseName);
        }


        public static Hashtable GetActiveObjectList(string filter)
        {
            Hashtable result = new Hashtable();

            IntPtr numFetched = IntPtr.Zero;
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            NativeMethods.GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();
            int clientNo = 0;
            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                NativeMethods.CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);
                System.Guid classId;
                monikers[0].GetClassID(out classId);

                object runningObjectVal;
                int res=runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                if (runningObjectName.IndexOf(filter) != -1)
                {
                    clientNo += 1;
                    result[runningObjectName] = runningObjectVal;
                }
            }

            return result;

        }

        public static object GetObjectFromRot(string monikerName, Guid guid)
        {
            IRunningObjectTable prot;
            IEnumMoniker ppenumMoniker;
            IntPtr pceltFetched = IntPtr.Zero;
            IMoniker[] rgelt = new IMoniker[1];
            object ppvResult = null;
            NativeMethods.GetRunningObjectTable(0, out prot);
            prot.EnumRunning(out ppenumMoniker);
            ppenumMoniker.Reset();
            while (ppenumMoniker.Next(1, rgelt, pceltFetched) == 0)
            {
                IBindCtx ppbc;
                string ppszDisplayName;
                NativeMethods.CreateBindCtx(0, out ppbc);
                rgelt[0].GetDisplayName(ppbc, null, out ppszDisplayName);
                if (!(!ppszDisplayName.StartsWith(monikerName) || ppszDisplayName.Equals(monikerName)))
                {
                    rgelt[0].BindToObject(ppbc, null, ref guid, out ppvResult);
                    return ppvResult;
                }
            }
            return ppvResult;
        }

        public static object GetActiveObject(string progId)
        {
            // Convert the prog id into a class id
            string classId = ConvertProgIdToClassId(progId);

            IRunningObjectTable prot = null;
            IEnumMoniker pMonkEnum = null;
            try
            {
                IntPtr Fetched = IntPtr.Zero;
                // Open the running objects table.
                NativeMethods.GetRunningObjectTable(0, out prot);
                prot.EnumRunning(out pMonkEnum);
                pMonkEnum.Reset();
                IMoniker[] pmon = new IMoniker[1];

                // Iterate through the results
                while (pMonkEnum.Next(1, pmon, Fetched) == 0)
                {
                    IBindCtx pCtx;

                    NativeMethods.CreateBindCtx(0, out pCtx);

                    string displayName;
                    pmon[0].GetDisplayName(pCtx, null, out displayName);

                    Marshal.ReleaseComObject(pCtx);
                    if (displayName.IndexOf(classId) != -1)
                    {
                        // Return the matching object
                        object objReturnObject;
                        int ret=prot.GetObject(pmon[0], out objReturnObject);
                        return objReturnObject;
                    }
                }
                return null;
            }
            finally
            {
                // Free resources
                if (prot != null)
                    Marshal.ReleaseComObject(prot);
                if (pMonkEnum != null)
                    Marshal.ReleaseComObject(pMonkEnum);
            }

        }


        public static Hashtable GetRunningObjectTable()
        {
            IRunningObjectTable prot;
            IEnumMoniker ppenumMoniker;
            IntPtr pceltFetched = IntPtr.Zero;
            Hashtable hashtable = new Hashtable();
            IMoniker[] rgelt = new IMoniker[1];
            NativeMethods.GetRunningObjectTable(0, out prot);
            prot.EnumRunning(out ppenumMoniker);
            ppenumMoniker.Reset();
            while (ppenumMoniker.Next(1, rgelt, pceltFetched) == 0)
            {
                IBindCtx ppbc;
                string ppszDisplayName;
                object ppunkObject;
                NativeMethods.CreateBindCtx(0, out ppbc);
                rgelt[0].GetDisplayName(ppbc, null, out ppszDisplayName);
                int ret=prot.GetObject(rgelt[0], out ppunkObject);
                hashtable[ppszDisplayName] = ppunkObject;
            }
            return hashtable;
        }

        public static string ConvertProgIdToClassId(string progID)
        {
            Guid testGuid;
            try
            {
                NativeMethods.CLSIDFromProgIDEx(progID, out testGuid);
            }
            catch
            {
                try
                {
                    NativeMethods.CLSIDFromProgID(progID, out testGuid);
                }
                catch
                {
                    return progID;
                }
            }
            return testGuid.ToString().ToUpper();
        }

        public static string ConvertClassIdToProgId(string classID)
        {
            Guid testGuid = new Guid(classID.Replace("!", ""));
            string progId = null;
            try
            {
                int res=NativeMethods.ProgIDFromCLSID(ref testGuid, out progId);
            }
            catch (Exception)
            {
                return null;
            }
            return progId;
        }

        public string GetServerName()
        {
            string serverName = "";
            int num = 0;
            num = this._objectDesigner.GetServerName(out serverName);
            if (serverName != null)
                return ConvertString(serverName);
            else
                return string.Empty;
        }

        private void ProcessResult(int result)
        {
            if (result != 0)
            {
                IErrorInfo ppIErrorInfo = null;
                int res=NativeMethods.GetErrorInfo(0, out ppIErrorInfo);
                string pBstrDescription = string.Empty;
                if (ppIErrorInfo != null)
                {
                    int res2=ppIErrorInfo.GetDescription(out pBstrDescription);
                }
                string message = string.Format(CultureInfo.CurrentCulture, "Method returned an error. HRESULT = 0x{0:X8}", new object[] { result });
                if (!String.IsNullOrEmpty(pBstrDescription))
                {
                    message = message + " : " + pBstrDescription;
                }
                throw new Exception(message);
            }
        }

        public void ReadObjectToFStream(NavObjectType navObjectType, int objectId, FileStream fStream)
        {
            IntPtr pLEN = Marshal.AllocHGlobal(4);
            IStream pOutStm;
            int res=NativeMethods.CreateStreamOnHGlobal(0, true, out pOutStm);

            String filter = "WHERE(Type=CONST(";//),ID=CONST(3))
            switch (navObjectType)
            {
                case NavObjectType.Table: filter += "Table"; break;
                case NavObjectType.Form: filter += "Form"; break;
                case NavObjectType.Report: filter += "Report"; break;
                case NavObjectType.Dataport: filter += "Dataport"; break;
                case NavObjectType.Codeunit: filter += "Codeunit"; break;
                case NavObjectType.XMLPort: filter += "XMLport"; break;
                case NavObjectType.Page: filter += "Page"; break;
                case NavObjectType.MenuSuite: filter += "MenuSuite"; break;
            }
            filter += "),ID=CONST(" + objectId.ToString() + "))";
            int result = this._objectDesigner.ReadObjects(filter, pOutStm);
            //int result = this._objectDesigner.ReadObject((int)navObjectType, objectId, pOutStm);

            pOutStm.Seek(0, 0, pLEN);
            int cnt, LEN = 4096;
            byte[] buffer = new byte[LEN];
            do
            {
                pOutStm.Read(buffer, LEN, pLEN);
                cnt = Marshal.ReadInt32(pLEN);
                fStream.Write(buffer, 0, cnt);
            }
            while (cnt == LEN);

            this.ProcessResult(result);
        }

        public void ReadObjectsToFStream(string filter, FileStream fStream)
        {
            IntPtr pLEN = Marshal.AllocHGlobal(4);
            IStream pOutStm;
            int res=NativeMethods.CreateStreamOnHGlobal(0, true, out pOutStm);

            int result = this._objectDesigner.ReadObjects(filter, pOutStm);

            pOutStm.Seek(0, 0, pLEN);
            int cnt, LEN = 4096;
            byte[] buffer = new byte[LEN];
            do
            {
                pOutStm.Read(buffer, LEN, pLEN);
                cnt = Marshal.ReadInt32(pLEN);
                fStream.Write(buffer, 0, cnt);
            }
            while (cnt == LEN);

            this.ProcessResult(result);
        }
        /**************
        public void WriteObjectFromStream(Stream stream)
        {
            IStream source = this.ToIStream(stream);
            int result = this._objectDesigner.WriteObjects(source);
            this.ProcessResult(result);
        }
        **********************/
    }
    /*
    public class MenuButton : INSMenuButton
    {
        #region INSMenuButton Members

        public int proc3(string a)
        {
            int result =
            Debug.WriteLine("INSMenuButton.proc3:"+a);
            return 1;
        }

        public int proc4(int a, string b, string c)
        {
            Debug.WriteLine("INSMenuButton.proc4: a=" + a+" b="+b+" c="+c);
            return 1;
        }

        #endregion
    }

    public class NAVApplication : INSApplication
    {
        #region INSApplication Members

        public int GetForm(out INSForm form)
        {
            
        }

        #endregion
    }
     * */
}
