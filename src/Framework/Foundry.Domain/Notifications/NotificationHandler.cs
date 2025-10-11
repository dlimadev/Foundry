namespace Foundry.Domain.Notifications
{
    /// <summary>
    /// The concrete implementation of the notification handler service.
    /// It's designed to be injected with a Scoped lifetime.
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        private readonly List<Notification> _notifications;
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
        public bool HasNotifications => _notifications.Any();
        
        // A new, more specific property to check only for critical errors.
        public bool HasErrors => _notifications.Any(n => n.Type == ENotificationType.Error);

        public NotificationHandler()
        {
            _notifications = new List<Notification>();
        }

        public void AddNotification(string key, string message, ENotificationType type)
        {
            _notifications.Add(new Notification(key, message, type));
        }
        
        // Helper method specifically for errors.
        public void AddError(string key, string message)
        {
            AddNotification(key, message, ENotificationType.Error);
        }
        
        // Helper method specifically for warnings.
        public void AddWarning(string key, string message)
        {
            AddNotification(key, message, ENotificationType.Warning);
        }
    }
}