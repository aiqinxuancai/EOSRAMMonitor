using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace CustomNotificationsExample.MahAppsNotification
{
    public class MahAppsNotification : NotificationBase, INotifyPropertyChanged
    {
        private MahAppsDisplayPart _displayPart;

        public override NotificationDisplayPart DisplayPart => _displayPart ?? (_displayPart = new MahAppsDisplayPart(this));

        public MahAppsNotification(string title, string message)
        {
            Title = title;
            Message = message;
        }

        #region binding properties
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
