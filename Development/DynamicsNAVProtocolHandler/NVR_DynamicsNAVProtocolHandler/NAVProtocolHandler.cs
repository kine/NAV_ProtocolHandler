using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Win32Helper;

namespace NVR_DynamicsNAVProtocolHandler
{
    class NAVProtocolHandler
    {
        static public void ProcessHandler(string uri)
        {
            try
            {
                uint pid = 0;
                try
                {
                    Win32Helper.Win32.User32.GetWindowThreadProcessId(Win32.User32.GetForegroundWindow(), out pid);
                    Process proc = System.Diagnostics.Process.GetProcessById((int)pid);
                    String activeProcess = proc.MainModule.FileName;
                    String path = Path.GetDirectoryName(activeProcess) + @"\";
                    //for attaching debugger
                    //if (MessageBox.Show("Attach the debugger...") == MessageBoxResult.OK) { }
                    if (Path.GetFileName(activeProcess).ToLower() == "finsql.exe")
                    {
                        if (File.Exists(path + "Microsoft.Dynamics.Nav.Client.exe"))
                        //Runs the client from same folder as calling process
                        {
                            RunProcess(path + "Microsoft.Dynamics.Nav.Client.exe", @"""" + uri + @"""");
                            return;
                        }

                        var versionInfo = FileVersionInfo.GetVersionInfo(activeProcess);
                        var fileVersion = versionInfo.FileVersion;
                        var navPath = FindNavClient(fileVersion, activeProcess);
                        if (String.IsNullOrEmpty(navPath))
                        {
                            MessageBox.Show("Error", "Same version of RTC was not found!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        var newUri = NAV_URI_Extender.GetExtendedUri(new Uri(uri), pid);

                        if (newUri != null)
                        {
                            RunProcess(navPath, @"""" + newUri.ToString() + @"""");
                        }
                        else
                        {
                            RunProcess(navPath, @"""" + uri + @"""");
                        }
                        return;
                    }
                    else
                    {
                        string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
                        RunProcess(defaultPath.Replace(@" ""%1""", ""), @"""" + uri + @"""");
                        return;
                        //Use Default
                    }
                }
                catch (Exception ex)
                {
                    string defaultPath = (String)Microsoft.Win32.Registry.GetValue(@"HKEY_CLASSES_ROOT\DYNAMICSNAV\Shell\Open\Command", "Default", "");
                    RunProcess(defaultPath.Replace(@" ""%1""", ""), @"""" + uri + @"""");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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

        static private void RunProcess(string path, string param)
        {
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
