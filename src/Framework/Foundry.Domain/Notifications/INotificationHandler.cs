namespace Foundry.Domain.Notifications
{
    /// <summary>
    /// Defines the contract for the notification handler service.
    /// </summary>
    public interface INotificationHandler
    {
        IReadOnlyCollection<Notification> Notifications { get; }
        bool HasNotifications { get; }
        bool HasErrors { get; }

        void AddNotification(string key, string message, ENotificationType type);
        void AddError(string key, string message);
        void AddWarning(string key, string message);
    }
}