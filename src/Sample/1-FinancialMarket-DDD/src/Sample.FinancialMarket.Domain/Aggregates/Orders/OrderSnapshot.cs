using Foundry.Domain.Model;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders
{
    public class OrderSnapshot : IEventSourcedSnapshot
    {
        public Guid AggregateId { get; init; }
        public int Version { get; init; }
        public string CustomerId { get; init; } = string.Empty;
        public EOrderType OrderType { get; init; }
        public string Status { get; init; } = string.Empty;
        public List<OrderLineItem> LineItems { get; init; } = new();
    }
}