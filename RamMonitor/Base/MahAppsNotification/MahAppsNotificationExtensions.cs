using ToastNotifications;

namespace CustomNotificationsExample.MahAppsNotification
{
    public static class MahAppsNotificationExtensions
    {
        public static void ShowMahAppsNotification(this Notifier notifier, string title, string message)
        {
            notifier.Notify<MahAppsNotification>(() => new MahAppsNotification(title, message));
        }
    }
}
