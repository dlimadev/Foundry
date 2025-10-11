using Foundry.Domain.Notifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Services
{
    /// <summary>
    /// A Domain Service contract that defines the capability of validating an order before opening.
    /// This abstraction allows the Order entity to trigger its validation logic (which may be complex,
    /// like a Chain of Responsibility) without being directly coupled to the implementation of that logic.
    /// </summary>
    public interface IOrderValidationService
    {
        /// <summary>
        /// Validates the provided order to ensure it meets all business criteria to be opened.
        /// </summary>
        /// <param name="order">The order aggregate to validate.</param>
        /// <param name="notifier">The notification handler to collect any validation errors found during the process.</param>
        Task ValidateForOpening(Order order, INotificationHandler notifier);
    }
}