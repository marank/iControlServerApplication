using System;

namespace iControlServerApplication {
    public class iControlNotificationManager {

        private TrayIcon trayIcon;
        private iControlNotifications _notifications;

        public iControlNotificationManager() {
            _notifications = new iControlNotifications();
            this.trayIcon = new TrayIcon();
            trayIcon.Display();
        }

        public void ShowNotfication(string caption, string message) {
            if ((Boolean)Program.GetSetting("notifications", true) == false) return;

            if (true) {
                trayIcon.ShowBalloonTip(5, caption, message, System.Windows.Forms.ToolTipIcon.Info);
            } else {
                iControlNotification noti = new iControlNotification(caption, message);
                noti.Show();
                _notifications.Add(noti);
            }
        }

        public void Shutdown() {
            foreach (iControlNotification noti in _notifications) {
                noti.Close();
            }
            trayIcon.Instance.Visible = false;
            trayIcon.Dispose();
        }
    }
}
