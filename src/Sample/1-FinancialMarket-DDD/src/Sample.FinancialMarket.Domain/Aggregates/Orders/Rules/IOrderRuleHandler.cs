using Foundry.Domain.Notifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Rules
{
    /// <summary>
    /// Defines the contract for a handler in the Chain of Responsibility pattern
    /// that executes a specific business rule.
    /// </summary>
    public interface IOrderRuleHandler
    {
        IOrderRuleHandler SetNext(IOrderRuleHandler handler);
        Task Handle(Order order, INotificationHandler notifier);
    }
}