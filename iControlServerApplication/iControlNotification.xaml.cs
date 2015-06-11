using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;

namespace iControlServerApplication {
    /// <summary>
    /// Interaktionslogik für iControlNotificationView.xaml
    /// </summary>
    public partial class iControlNotification : Window {

        private iControlNotificationModel _notification;

        public iControlNotification(string caption, string message) {
            InitializeComponent();

            _notification = new iControlNotificationModel() {
                Caption = caption,
                Message = message
            };

            this.DataContext = _notification;
        }

        public new void Show() {
            this.Topmost = true;
            base.Show();

            var workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Left = workingArea.Right - this.ActualWidth - 10;
            this.Top = workingArea.Bottom - this.ActualHeight - 10;
        }
    }

    public class iControlNotifications : ObservableCollection<iControlNotification> { }
}
