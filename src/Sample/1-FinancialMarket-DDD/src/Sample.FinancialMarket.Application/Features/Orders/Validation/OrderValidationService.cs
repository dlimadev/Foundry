using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Rules.Handlers;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;

namespace Sample.FinancialMarket.Application.Features.Orders.Validation
{
    /// <summary>
    /// Concrete implementation of the IOrderValidationService domain service.
    /// This is where we build and execute the Chain of Responsibility for order validation.
    /// </summary>
    public class OrderValidationService : IOrderValidationService
    {
        public async Task ValidateForOpening(Order order, INotificationHandler notifier)
        {
            // Now the compiler can find these classes
            var validationChain = new CreditRiskHandler();
            validationChain.SetNext(new ComplianceHandler());

            await validationChain.Handle(order, notifier);
        }
    }
}