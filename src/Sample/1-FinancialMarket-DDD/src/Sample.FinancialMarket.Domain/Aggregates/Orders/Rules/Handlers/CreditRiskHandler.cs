using Foundry.Domain.Notifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Rules.Handlers
{
    public class CreditRiskHandler : BaseOrderValidationHandler
    {
        private const decimal HighRiskThreshold = 50_000m;

        public override async Task Handle(Order order, INotificationHandler notifier)
        {
            if (order.TotalValue.Amount > HighRiskThreshold)
            {
                notifier.AddError("order.creditRisk.exceeded", $"Order value ({order.TotalValue.Amount:C}) exceeds the high-risk threshold of {HighRiskThreshold:C}.");
                return;
            }

            if (NextHandler != null)
            {
                await NextHandler.Handle(order, notifier);
            }
        }
    }
}