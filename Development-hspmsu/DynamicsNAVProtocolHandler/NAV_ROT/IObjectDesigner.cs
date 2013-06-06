using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NAV_ROT
{
    [ComImport, Guid("50000004-0000-1000-0001-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectDesigner
    {
        [PreserveSig, DispId(1)]
        int ReadObject([In] int objectType, [In] int objectId, [In] IStream destination);
        [PreserveSig, DispId(2)]
        int ReadObjects([In] string filter, [In] IStream destination);
        [PreserveSig, DispId(3)]
        int WriteObjects([In] IStream source);
        [PreserveSig, DispId(4)]
        int CompileObject([In] int objectType, [In] int objectId);
        [PreserveSig, DispId(5)]
        int CompileObjects([In] string filter);
        [PreserveSig, DispId(6)]
        int GetServerName(out string serverName);
        [PreserveSig, DispId(7)]
        int GetDatabaseName(out string databaseName);
        [PreserveSig, DispId(8)]
        int GetServerType(out int serverType);
        [PreserveSig, DispId(9)]
        int GetCSIDEVersion(out string csideVersion);
        [PreserveSig, DispId(10)]
        int GetApplicationVersion(out string applicationVersion);
        [PreserveSig, DispId(11)]
        int GetCompanyName(out string companyName);
    }

    [ComImport, Guid("50000004-0000-1000-0011-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSCallbackEnum
    {
        int NextRecord([In, MarshalAs(UnmanagedType.Interface)] INSRec record);
        int NextFieldValue([In] int fieldo, [In] string fieldValue, [In] string dataType);
        int NextFilterValue([In] int fieldNo, [In] string filterValue);
        int NextTable([In] int tableNo, [In] string tableName);
        int NextFieldDef([In] int fieldNo, [In] string fieldName, [In] string fieldCaption, [In] string dataType, [In] int dataLength, [In] int f);
    }

    [ComImport, Guid("50000004-0000-1000-0007-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSRec
    {
        int SetFieldValue([In] int fieldNo, [In] String value, [In] int validate);
        int GetFieldValue([In] int fieldNo, out String value);
        int EnumFieldValues([In, MarshalAs(UnmanagedType.Interface)] INSCallbackEnum callback);
    }

    [ComImport, Guid("50000004-0000-1000-0006-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSTable
    {
        int Delete([In, MarshalAs(UnmanagedType.Interface)] INSRec rec);
        int Insert([In, MarshalAs(UnmanagedType.Interface)] INSRec rec);
        int Modify([In, MarshalAs(UnmanagedType.Interface)] INSRec rec);
        int Init([Out, MarshalAs(UnmanagedType.Interface)] out INSRec rec);
        int SetFilter([In] int fieldNo, [In] string value);
        int EnumFilters([In, MarshalAs(UnmanagedType.Interface)] INSCallbackEnum callback);
        int EnumRecords([In, MarshalAs(UnmanagedType.Interface)] INSCallbackEnum callback);
        int EnumFields([In, MarshalAs(UnmanagedType.Interface)] INSCallbackEnum callback, [In] int languageI);
        int Find([In, MarshalAs(UnmanagedType.Interface)] INSRec rec);
        int GetID(out int tableNo);
        int proc13(out int a);
    }

    [ComImport, Guid("50000004-0000-1000-0010-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSAppBase
    {
        int GetTable([In] int tableNo, [Out, MarshalAs(UnmanagedType.Interface)] out INSTable table);
        int GetInfos(out string servername, out string databasename, out string company, out string username);
        int StartTrans();
        int proc6([In] bool a);
        int Error([In] string message);
        int EnumTables([In, MarshalAs(UnmanagedType.Interface)] INSCallbackEnum callback, [In] int flag);
    }

    [ComImport, Guid("50000004-0000-1000-0005-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSHook
    {
        /*
         *  OUT LONG [#8] [proc#3] ( IN MS IID{INSAppBase} [#4] )
         * */
        int proc3([In, MarshalAs(UnmanagedType.Interface)] INSAppBase appBase);
    }

    [ComImport, Guid("50000004-0000-1000-0009-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSMenuButtonEvents
    {
        int proc3();
        int proc4([In] int a);
    }

    [ComImport, Guid("50000004-0000-1000-0004-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSApplicationEvents
    {
        int OnFormOpen([In,MarshalAs(UnmanagedType.Interface)] INSForm form);
        int proc4([In, MarshalAs(UnmanagedType.Interface)] INSForm form, [In] String b);
        int OnActiveChanged([In] bool active);
        int OnCompanyClose();
    }

    [ComImport, Guid("50000004-0000-1000-0000-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSHyperlink
    {
        int Open([In] string link);
        int GetNavWindowHandle(out int handle);
    }

    [ComImport, Guid("50000004-0000-1000-0008-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSMenuButton
    {
        int proc3([In] string a);
        int proc4([In] int a, [In] string b, [In] string c);
    }

    [ComImport, Guid("50000004-0000-1000-0003-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSForm
    {
        int GetHyperlink(out String link);        // gets Hyperlink with Fieldcaptions
        int GetID(out String formID);               // gets active object's type (Form) and ID
        int GetRec([Out, MarshalAs(UnmanagedType.Interface)] out INSRec recod);
        int GetTable([Out, MarshalAs(UnmanagedType.Interface)] out INSTable table);
        int GetLanguageID(out int languageID);          // gets Language ID of application (1033, etc.)
        int GetButton([Out, MarshalAs(UnmanagedType.Interface)] out INSMenuButton menuButton);
        int proc9();
    }

    [ComImport, Guid("50000004-0000-1000-0002-0000836BD2D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INSApplication
    {
        int GetForm([Out, MarshalAs(UnmanagedType.Interface)] out INSForm form);
    }

}

