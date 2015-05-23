﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using iControlPluginInterface;

namespace iControlServerApplication {
    static class Program {

        static TrayIcon trayIcon;
        static TCPServer server;
        static IiControlPlugin[] plugins;
        static iControlPluginHost pluginHost;

        [STAThread]
        static void Main() {
            Log("iControlServerApplication started.");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            trayIcon = new TrayIcon();
            trayIcon.Display();

            pluginHost = new iControlPluginHost();
            LoadPlugins();

            server = new TCPServer();
            server.CommandReceived += new TCPServer.CommandReceivedEventHandler(tcpServer_CommandReceived);
            server.Start();

            trayIcon.Instance.ShowBalloonTip(5, "iControl Server Application", "Server started. Listening for clients.", ToolTipIcon.Info);

            Application.Run();
        }

        static void LoadPlugins() {
            string path = Application.StartupPath + "\\plugins";
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            string[] pluginFiles = System.IO.Directory.GetFiles(path, "*.dll");
            plugins = new IiControlPlugin[pluginFiles.Length];

            for (int i = 0; i < pluginFiles.Length; i++) {
                string args = pluginFiles[i].Substring(
                    pluginFiles[i].LastIndexOf("\\") + 1,
                    pluginFiles[i].IndexOf(".dll") - pluginFiles[i].LastIndexOf("\\") - 1);

                Type icpt = null;
                try {
                    System.Reflection.Assembly assembly = null;
                    assembly = System.Reflection.Assembly.LoadFile(pluginFiles[i]);
                    if (assembly != null) {
                        icpt = assembly.GetType(args + ".iControlPlugin");
                    }
                } catch (Exception ex) {
                    Log(ex.Message);
                }
                try {
                    if (icpt != null) {
                        IiControlPlugin plugin = (IiControlPlugin)Activator.CreateInstance(icpt);
                        plugin.Host = pluginHost;
                        if (plugin.Init() == true) {
                            plugins[i] = plugin;
                        }
                    }
                } catch (Exception ex) {
                    Log(ex.Message);
                }

                Log("Plugin loaded: " + plugins[i].Name + " by " + plugins[i].Author);
            }
        }

        static void tcpServer_CommandReceived(object source, TCPServer.CommandReceivedEventArgs e) {
            string toolTipText = "[" + e.Client.IPAddress + "] >> " + e.Command;
            trayIcon.Instance.ShowBalloonTip(5, "iControl Server Application", toolTipText, ToolTipIcon.Info);

            foreach (IiControlPlugin plugin in plugins) {
                plugin.Handle(e.SplittedCommands, e.Client.IPAddress);
            }
        }

        static public void Exit() {
            server.Stop();
            trayIcon.Instance.Visible = false;
            trayIcon.Dispose();
            Log("iControlServerApplication stopped.");
            Application.Exit();
        }

        static public void Log(string msg) {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\applicationlog.txt";
            string text = String.Format("[{0:s}] {1}", System.DateTime.Now, msg) + Environment.NewLine;

            System.IO.File.AppendAllText(path, text);
        }
    }
}