namespace Foundry.Domain.Notifications
{
    /// <summary>
    /// Represents a single validation notification or message, now with a severity type.
    /// </summary>
    public class Notification
    {
        public string Key { get; }
        public string Message { get; }
        public ENotificationType Type { get; }

        public Notification(string key, string message, ENotificationType type)
        {
            Key = key;
            Message = message;
            Type = type;
        }
    }
}