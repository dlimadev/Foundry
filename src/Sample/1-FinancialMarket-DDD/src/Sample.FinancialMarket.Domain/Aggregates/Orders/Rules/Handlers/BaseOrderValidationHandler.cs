using Foundry.Domain.Notifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Rules.Handlers
{
    public abstract class BaseOrderValidationHandler : IOrderRuleHandler
    {
        protected IOrderRuleHandler? NextHandler;

        public IOrderRuleHandler SetNext(IOrderRuleHandler handler)
        {
            NextHandler = handler;
            return handler;
        }

        public abstract Task Handle(Order order, INotificationHandler notifier);
    }
}