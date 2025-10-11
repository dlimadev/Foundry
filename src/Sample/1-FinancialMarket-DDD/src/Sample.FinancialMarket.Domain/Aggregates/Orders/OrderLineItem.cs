// Replace your existing OrderLineItem class with this complete version.
using Foundry.Domain.Model;
using Sample.FinancialMarket.Domain.Common.ValueObjects;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders
{
    public class OrderLineItem : EntityBase
    {
        public Guid OrderId { get; private set; }
        public string Ticker { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public Money Price { get; private set; } = Money.Create(0, "EUR");

        private OrderLineItem() { }

        /// <summary>
        /// Internal factory method to create a new line item.
        /// Can only be called from within the Domain assembly, enforcing the Aggregate boundary.
        /// </summary>
        internal static OrderLineItem Create(Guid orderId, string ticker, int quantity, Money price)
        {
            // Validations for the line item itself can go here
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));

            return new OrderLineItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Ticker = ticker,
                Quantity = quantity,
                Price = price
            };
        }
    }
}