using Foundry.Domain.Notifications;
using System.Net;

namespace Foundry.Application.Abstractions.Responses
{
    public class Result<T>
    {
        public bool IsSuccess => !Notifications.Any(n => n.Type == ENotificationType.Error);
        public T? Value { get; }
        public IReadOnlyCollection<Notification> Notifications { get; }
        public HttpStatusCode? SuggestedStatusCode { get; }

        private Result(T? value, IReadOnlyCollection<Notification> notifications, HttpStatusCode? suggestedStatusCode = null)
        {
            Value = value;
            Notifications = notifications;
            SuggestedStatusCode = suggestedStatusCode;
        }

        public static Result<T> Success(T value, HttpStatusCode? statusCode = null, IReadOnlyCollection<Notification>? notifications = null)
            => new(value, notifications ?? new List<Notification>().AsReadOnly(), statusCode);

        public static Result<T> Failure(IReadOnlyCollection<Notification> notifications, HttpStatusCode? statusCode = null)
        {
            if (!notifications.Any(n => n.Type == ENotificationType.Error))
                throw new ArgumentException("A failure result must contain at least one error notification.", nameof(notifications));

            return new Result<T>(default, notifications, statusCode);
        }
    }
}