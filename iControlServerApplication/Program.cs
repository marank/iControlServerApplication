using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using iControlInterfaces;

namespace iControlServerApplication {
    static class Program {

        static TrayIcon trayIcon;
        static TCPServer server;
        static List<IiControlPlugin> plugins;
        static iControlPluginHost pluginHost;
        static Dictionary<string, object> Settings;
        static public string ApplicationName = "iControlServerApplication";
        static public string DataDir;
        static public Boolean Autostart {
            get {
                Microsoft.Win32.RegistryKey root = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                return ((string)root.GetValue(ApplicationName) == Application.ExecutablePath.ToString());
            }
            set {
                Microsoft.Win32.RegistryKey root = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value) {
                    root.SetValue(ApplicationName, Application.ExecutablePath.ToString());
                    root.Close();
                } else {
                    root.DeleteValue(ApplicationName);
                    root.Close();
                }
            }
        }

        static public List<IiControlPlugin> Plugins {
            get { return plugins; }
        }

        [STAThread]
        static void Main() {
            Microsoft.Win32.RegistryKey root = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\iControlServer", true);
            DataDir = (string)root.GetValue("DataDir");

            Log("iControlServerApplication started.");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadSettings();

            plugins = new List<IiControlPlugin>();
            pluginHost = new iControlPluginHost();
            LoadPlugins();

            trayIcon = new TrayIcon();
            trayIcon.Display();

            server = new TCPServer();
            server.CommandReceived += new TCPServer.CommandReceivedEventHandler(tcpServer_CommandReceived);
            if (server.Start()) {
                trayIcon.ShowBalloonTip(5, "iControl Server Application", "Server started. " + plugins.Count + " plugins loaded.", ToolTipIcon.Info);
                Application.Run();
            } else {
                MessageBox.Show("Port " + server.Port + " is already in use. Server could not be started.", ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }      
        }

        static void LoadPlugins() {
            Log("Loading plugins...");
            
            string path = System.IO.Path.Combine(DataDir, "plugins");
            Log("Plugin Directory: " + path);
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            string[] pluginFiles = System.IO.Directory.GetFiles(path, "*.dll");

            foreach (string file in pluginFiles) {
                try {
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (Type type in assembly.GetTypes()) {
                        if (type.IsClass && type.IsPublic && !type.IsAbstract) {
                            Type typeInterface = type.GetInterface(typeof(IiControlPlugin).ToString(), true);
                            if (typeInterface != null) {
                                IiControlPlugin plugin = (IiControlPlugin)Activator.CreateInstance(type);
                                Log(String.Format("[{0}] Plugin loaded: {1} ({2}) by {3}", System.IO.Path.GetFileName(file), plugin.Name, plugin.Version, plugin.Author));
                                plugin.Host = pluginHost;
                                if (plugin.Init() == true) {
                                    plugins.Add(plugin);
                                } else {
                                    Log(String.Format("[{0}] Plugin disabled", System.IO.Path.GetFileName(file)));
                                }
                            }
                            typeInterface = null;
                        }
                    }
                } catch (Exception ex) {
                    Log(String.Format("[{0}] {1}: {2}", System.IO.Path.GetFileName(file), ex.ToString(), ex.Message));
                }
            }
            Log(plugins.Count + " plugins loaded.");
        }

        static void tcpServer_CommandReceived(object source, TCPServer.CommandReceivedEventArgs e) {
            string toolTipText = "[" + e.Client.IPAddress + "] >> " + e.Command;
            trayIcon.ShowBalloonTip(5, "iControl Server Application", toolTipText, ToolTipIcon.Info);

            foreach (IiControlPlugin plugin in plugins) {
                plugin.Handle(e.SplittedCommands, e.Client);
            }
        }

        static void LoadSettings() {
            string path = System.IO.Path.Combine(DataDir, ApplicationName + ".config");
            Log("Trying to load settings from file: " + path);
            if (System.IO.File.Exists(path)) {
                Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(System.IO.File.ReadAllText(path));
                Log("Settings successfully loaded.");
            } else {
                Settings = new Dictionary<string, object>();
                Log("Failed loading settings: File does not exists. Default settings are being used.");
            }
        }

        static public object GetSetting(string key, object value) {
            if (Settings.ContainsKey(key)) {
                value =  Settings[key];
            }

            return value;
        }

        static public void SetSetting(string key, object value) {
            if (Settings.ContainsKey(key)) {
                Settings[key] = value;
            } else {
                Settings.Add(key, value);
            }

            string path = System.IO.Path.Combine(DataDir, ApplicationName + ".config");
            System.IO.File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(Settings, Newtonsoft.Json.Formatting.Indented));
        }

        static public void Exit() {
            server.Stop();
            trayIcon.Instance.Visible = false;
            trayIcon.Dispose();
            Log("iControlServerApplication stopped.");
            Application.Exit();
        }

        static public Boolean ToggleAutostart() {
            Autostart = !Autostart;
            return Autostart;
        }

        static public void Log(string msg) {
            WriteLog(String.Format("[{0}] {1}", "Main", msg));
        }

        static public void WriteLog(string msg) {
            string path = System.IO.Path.Combine(DataDir, "application.log");
            string text = String.Format("[{0:s}] {1}", System.DateTime.Now, msg) + Environment.NewLine;

            System.IO.File.AppendAllText(path, text);
        }
    }
}
