using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public string CustomerId { get; }
        public OrderCreatedEvent(Guid orderId, string customerId)
        {
            OrderId = orderId;
            CustomerId = customerId;
        }
    }
}