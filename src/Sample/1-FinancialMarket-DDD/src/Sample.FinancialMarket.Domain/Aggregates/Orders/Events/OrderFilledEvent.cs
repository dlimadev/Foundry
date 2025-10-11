using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Events
{
    public class OrderFilledEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public DateTime FilledDate { get; }
        public OrderFilledEvent(Guid orderId, DateTime filledDate)
        {
            OrderId = orderId;
            FilledDate = filledDate;
        }
    }
}