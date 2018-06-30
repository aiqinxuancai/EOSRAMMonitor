using ToastNotifications.Core;

namespace CustomNotificationsExample.MahAppsNotification
{
    /// <summary>
    /// Interaction logic for MahAppsDisplayPart.xaml
    /// </summary>
    public partial class MahAppsDisplayPart : NotificationDisplayPart
    {
        private MahAppsNotification _notification;

        public MahAppsDisplayPart(MahAppsNotification notification)
        {
            InitializeComponent();

            _notification = notification;
            DataContext = notification;
        }
    }
}
