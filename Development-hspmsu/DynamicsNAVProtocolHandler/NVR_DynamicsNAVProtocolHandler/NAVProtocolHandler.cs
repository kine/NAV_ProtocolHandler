using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using Win32Helper;

namespace NVR_DynamicsNAVProtocolHandler
{
    class NAVProtocolHandler
    {
        /// <summary>
        /// Handler for the DynamicsNAV protocol. Called instead original handler. Based on the uri and last forward NAV client will
        /// decide the version of RTC to open and do the rest to open the URL.
        /// </summary>
        /// <param name="uri">The DynamicsNAV protocol URI.</param>
        static public void ProcessHandler(string uri)
        {
            try
            {
                uint pid = 0;
                try
                {
                    Win32Helper.Win32.User32.GetWindowThreadProcessId(Win32.User32.GetForegroundWindow(), out pid);
                    if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.Debug)
                    {
                        if (MessageBox.Show("Attach the debugger...") == MessageBoxResult.OK) { }
                    }
                    Process proc = System.Diagnostics.Process.GetProcessById((int)pid);
                    String activeProcessFolder = proc.MainModule.FileName;
                    String path = Path.GetDirectoryName(activeProcessFolder) + @"\";
                    //for attaching debugger
                    if ((Path.GetFileName(activeProcessFolder).ToLower() == "finsql.exe") ||
                        (Path.GetFileName(activeProcessFolder).ToLower() == "Microsoft.Dynamics.Nav.Client.exe"))
                    { //Known client, take the version from the client
                        RunFromClient(uri, pid, activeProcessFolder, path);
                        return;
                    }
                    else
                    {//Unknown client, check mapping
                        return;
                        //Use Default
                    }
                }
                catch (Exception ex)
                {
                    //string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
                    RunFromUnknown(uri);
                    //RunProcess(defaultPath, @"""" + uri + @"""");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// The URI was fired from unknown client (outside NAV client).
        /// </summary>
        /// <param name="uri">The URI.</param>
        private static void RunFromUnknown(string uri)
        {
            var fileVersion = NAV_URI_Extender.GetVersionFromMapping(uri);
            if (fileVersion == null)
            {
                string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
                string parameters = @"""" + uri + @"""";

                if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.CreateSettingsFile)
                {
                    string settingsfile = prepareCUS(uri);
                    if (!String.IsNullOrEmpty(settingsfile))
                        parameters = @"-settings:""" + settingsfile + @""" " + parameters;
                }
                RunProcess(defaultPath, parameters);
                return;
            }
            foreach (String navVersionFolder in GetNavVersionFolders()) {
                if (RunForVersion(uri, navVersionFolder, fileVersion, false))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the nav version folders from registry.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetNavVersionFolders()
        {
            var folders = new List<String>();
            foreach (String registryPath in new List<String>(NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.NavVersionRegistryKeys.Split(';')))
            {
                try
                {
                    var folder = (String)Microsoft.Win32.Registry.GetValue(registryPath.Replace(System.Environment.NewLine, ""), "Path", "");
                    folders.Add(folder);
                }
                catch (Exception e)
                {
                }
            }
            return folders;
        }

        /// <summary>
        /// The URI was fired from NAV client.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="pid">The NAV client pid.</param>
        /// <param name="activeProcessFolder">The active process folder.</param>
        /// <param name="path">The path.</param>
        private static void RunFromClient(string uri, uint pid, String activeProcessFolder, String path)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(activeProcessFolder);
            var fileVersion = versionInfo.FileVersion;
            if (pid != 0)
            {
                var newUri = NAV_URI_Extender.GetExtendedUri(new Uri(uri), pid);
                if (newUri != null)
                {
                    uri = newUri.ToString();
                }
            }

            NAVClient2URI.UpdateVersion(uri, fileVersion);
            if (File.Exists(path + "Microsoft.Dynamics.Nav.Client.exe"))
            //Runs the client from same folder as calling process
            {
                string parameters = @"""" + uri + @"""";

                if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.CreateSettingsFile)
                {
                    string settingsfile = prepareCUS(uri);
                    if (!String.IsNullOrEmpty(settingsfile))
                        parameters = @"-settings:""" + settingsfile + @""" " + parameters;
                }

                RunProcess(path + "Microsoft.Dynamics.Nav.Client.exe", parameters);
                return;
            }

            RunForVersion(uri, activeProcessFolder, fileVersion);
        }

        /// <summary>
        /// Runs the URI for specified version.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="pid">The pid.</param>
        /// <param name="activeProcessFolder">The active process folder.</param>
        /// <param name="fileVersion">The file version.</param>
        /// <param name="showMessageIfNotFound">if set to <c>true</c> [show message if not found].</param>
        /// <returns></returns>
        private static bool RunForVersion(string uri, String activeProcessFolder, string fileVersion, bool showMessageIfNotFound=true)
        {
            var navPath = FindNavClient(fileVersion, activeProcessFolder);
            if (String.IsNullOrEmpty(navPath))
            {
                if (!showMessageIfNotFound)
                {
                    MessageBox.Show("Version " + fileVersion + " of RTC was not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }
            string parameters = @"""" + uri + @"""";

            if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.CreateSettingsFile)
            {
                string settingsfile = prepareCUS(uri);
                if (!String.IsNullOrEmpty(settingsfile))
                    parameters = @"-settings:""" + settingsfile + @""" " + parameters;
            }
            RunProcess(navPath, parameters);
            return true;
        }

        /// <summary>
        /// Traverses the folder tree to find correct version of NAV client.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="_navClientEXEList">The _nav client EXE list.</param>
        /// <param name="fileVersion">The file version.</param>
        /// <exception cref="System.ArgumentException"></exception>
        static private void TraverseTree(string root, string filter, List<string> _navClientEXEList, string fileVersion)
        {
            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    //Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    //Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir, filter);
                }

                catch (UnauthorizedAccessException e)
                {

                    //Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    //Console.WriteLine(e.Message);
                    continue;
                }
                // Perform the required action on each file here.
                // Modify this block to perform your required task.
                foreach (string file in files)
                {
                    try
                    {
                        string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(file).FileVersion;
                        if (fileVersion == version)
                        {
                            _navClientEXEList.Add(file);
                        }
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // If file was deleted by a separate application
                        //  or thread since the call to TraverseTree()
                        // then just continue.
                        //Console.WriteLine(e.Message);
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
        }


        /// <summary>
        /// Finds the nav client with correct version.
        /// </summary>
        /// <param name="fileVersion">The file version.</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <returns></returns>
        static private string FindNavClient(string fileVersion, string currentFolder)
        {
            string fileName = "";
            var progress = new Progress();
            progress.Show();
            progress.Topmost = true;
            try
            {
                var root = Directory.GetParent(currentFolder).Parent;
                List<String> files = new List<string>();
                TraverseTree(root.FullName, "Microsoft.Dynamics.Nav.Client.exe", files, fileVersion);
                if (files.Count > 0)
                    return files[0];
            }
            catch (Exception e)
            {
            }
            finally
            {
                progress.Close();
            }
            return fileName;
        }

        /// <summary>
        /// Create a valid CustomUserSettings.config for the given url to satisfy the RTC and avoid security warning dialog.
        /// </summary>
        /// <param name="path">The URI.</param>
        private static string prepareCUS(string url)
        {
            // Skip settingsfile preparation if this feature is disabled by our config
            if (!NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.CreateSettingsFile)
                return "";

            // get the default path to the std. CustomUserSettings.config
            // should be c:\programdata\Microsoft\Microsoft Dynamics NAV for both 2009 and 2013
            String stdCUS = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Microsoft\\Microsoft Dynamics NAV";

            // get the user appdata path to save the prepared CustomUserSettings.config
            String userCUS = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Microsoft Dynamics NAV";

            // parse the URL and store the values of the properties we need
            Hashtable props = new Hashtable();

            // if the url contains server:port
            if (url.Split('/')[2].Contains(":"))
            {
                props.Add("Server", url.Split('/')[2].Split(':')[0]);
                props.Add("ServerPort", url.Split('/')[2].Split(':')[1]);
            }
            else
            {
                // the port is missing, set the default value from config
                props.Add("Server", url.Split('/')[2]);
                props.Add("ServerPort", NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.NAVServerDefaultPort);
            }
            props.Add("ServerInstance", url.Split('/')[3]);
            // clear "predefined" values
            props.Add("UrlHistory", "");
            props.Add("UnknownSpnHint", "");

            FileInfo CUS = new FileInfo(stdCUS + "\\ClientUserSettings.config");

            // generate the full path for the prepared customusersettings.config
            FileInfo preparedCUS = new FileInfo(userCUS + "\\" + props["Server"] + "_" + props["ServerInstance"] + ".config");

            // if the file already exists return the path of it
            if (preparedCUS.Exists)
                return preparedCUS.FullName;

            if (CUS != null)
            {
                if (CUS.Exists)
                {
                    try
                    {
                        //Create the new CUS from the stdCUS.
                        XmlDocument doc = new XmlDocument();
                        doc.Load(CUS.FullName);
                        XmlNode node = doc.SelectSingleNode("//appSettings");

                        foreach (string parameter in new string[] { "UnknownSpnHint", "UrlHistory", "ServerPort", "ServerInstance", "Server" })
                        {
                            XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", parameter));

                            if (elem != null)
                            {
                                // get the property from the hashtable and set the value for the given key 
                                elem.SetAttribute("value", (string)props[parameter]);
                            }
                            else
                            {
                                string tempstr = (string)props[parameter];
                                // key was not found so create the 'add' element
                                // and set it's key/value attributes
                                elem = doc.CreateElement("add");
                                elem.SetAttribute("key", parameter);
                                elem.SetAttribute("value", tempstr);
                                node.AppendChild(elem);
                            }
                        }

                        // on a clean freshly installed system this path does not exist, so we need to create it
                        // to avoid the exception "parts of the path do not exist"
                        if (!Directory.Exists(userCUS))
                            Directory.CreateDirectory(userCUS);

                        // save the NEW CUS at the generated path
                        doc.Save(preparedCUS.FullName);
                    }
                    catch (Exception ex)
                    {
                        if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.Debug)
                        {
                            if (MessageBox.Show("Exception ...\n" + ex.Message, "[prepCUS]") == MessageBoxResult.OK) { }
                        }
                        // something bad happend, skip the settings
                        return "";
                    }
                }
            }
            return preparedCUS.FullName;
        }

        /// <summary>
        /// Runs the NAV client.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="param">The param.</param>
        static private void RunProcess(string path, string param)
        {
            path = path.Replace(@" ""%1""", "");
            if (path.IndexOf("-protocolhandler") > 0)
            {
            path = path.Replace(@" -protocolhandler", "");
                param = @"-protocolhandler " + param;
            }

            var procInfo = new ProcessStartInfo(path, param);
            procInfo.WorkingDirectory = Path.GetDirectoryName(path);
            //procInfo.FileName = Path.GetFileName(path);
            if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.ShowMessageBox)
            {
                if (MessageBox.Show("Starting this command:\n" + procInfo.FileName + @" " + param) == MessageBoxResult.OK) { }
            }

            var baloon = new Toaster(path);
            if (NVR_DynamicsNAVProtocolHandler.Properties.Settings.Default.ShowFileInfo)
            {
                baloon.Show();
            }
            Process.Start(procInfo);
        }

    }
}
