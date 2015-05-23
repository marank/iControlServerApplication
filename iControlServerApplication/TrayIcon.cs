using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace iControlServerApplication {
    class TrayIcon : IDisposable {

        NotifyIcon trayIcon;
        public NotifyIcon Instance { get { return trayIcon; } }

        public TrayIcon() {
            this.trayIcon = new NotifyIcon();
        }

        public void Display() {
            this.trayIcon.MouseClick += new MouseEventHandler(trayIcon_MouseClick);
            this.trayIcon.Icon = Properties.Resources.tray;
            this.trayIcon.Text = "iControl Server Application";
            this.trayIcon.Visible = true;

            this.trayIcon.ContextMenuStrip = new iControlContextMenu().Create();
        }

        public void Dispose() {
            this.trayIcon.Dispose();
        }

        void trayIcon_MouseClick(object sender, MouseEventArgs e) {
            //if (e.Button == MouseButtons.Left) {
            //    Process.Start("explorer", null);
            //}
        }
    }
}
