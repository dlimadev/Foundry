using Foundry.Domain.Notifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Rules.Handlers
{
    public class ComplianceHandler : BaseOrderValidationHandler
    {
        private static readonly List<string> RestrictedTickers = new() { "FORBIDDEN_INC", "BANNED_CO" };

        public override async Task Handle(Order order, INotificationHandler notifier)
        {
            var restrictedItem = order.LineItems.FirstOrDefault(item => RestrictedTickers.Contains(item.Ticker.ToUpper()));
            if (restrictedItem != null)
            {
                notifier.AddError("order.compliance.restrictedTicker", $"Trading for ticker '{restrictedItem.Ticker}' is restricted by compliance rules.");
                return;
            }

            if (NextHandler != null)
            {
                await NextHandler.Handle(order, notifier);
            }
        }
    }
}