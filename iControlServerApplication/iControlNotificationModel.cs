using System.ComponentModel;

namespace iControlServerApplication {
    public class iControlNotificationModel : INotifyPropertyChanged {

        private string _caption;
        public string Caption {
            get { return _caption; }
            set {
                if (value == _caption) return;

                _caption = value;
                OnPropertyChanged("Caption");
            }
        }

        private string _message;
        public string Message {
            get { return _message; }
            set {
                if (value == _message) return;

                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
