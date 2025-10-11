using Foundry.Domain.Exceptions;
using Foundry.Domain.Model;
using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Events;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;
using Sample.FinancialMarket.Domain.Aggregates.Orders.States;
using Sample.FinancialMarket.Domain.Common.ValueObjects;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders
{
    public class Order : EventSourcedAggregateRoot
    {
        public string CustomerId { get; private set; } = string.Empty;
        public EOrderType OrderType { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public string Status { get; private set; } = string.Empty;

        private readonly List<OrderLineItem> _lineItems = new();
        public IReadOnlyCollection<OrderLineItem> LineItems => _lineItems.AsReadOnly();
        public Money TotalValue => Money.Create(_lineItems.Sum(item => item.Price.Amount * item.Quantity), "EUR");

        private IOrderState _currentState = new PendingOrderState();

        public Order() { }

        public static Order Create(string customerId, EOrderType orderType, DateTime expirationDate)
        {
            var order = new Order();
            var evt = new OrderCreatedEvent(Guid.NewGuid(), customerId);
            order.ApplyAndStoreEvent(evt);

            order.OrderType = orderType;
            order.ExpirationDate = expirationDate;
            return order;
        }

        public async Task Open(IOrderValidationService validationService, INotificationHandler notifier)
        {
            await validationService.ValidateForOpening(this, notifier);
            if (notifier.HasErrors) return;
            _currentState.Open(this);
        }

        public void AddItem(string ticker, int quantity, Money price)
        {
            if (Status == nameof(EOrderStatus.Filled) || Status == nameof(EOrderStatus.Cancelled))
                throw new DomainException("orders.cannotModifyWhenClosed");

            if (_lineItems.Any(i => i.Ticker == ticker))
                throw new DomainException("orders.itemAlreadyExists", ticker);

            var newItem = OrderLineItem.Create(Id, ticker, quantity, price);
            _lineItems.Add(newItem);
        }

        public void ChangeState(IOrderState newState)
        {
            _currentState = newState;
            Status = newState.GetStatus().ToString();
        }

        private void Apply(OrderCreatedEvent evt)
        {
            Id = evt.OrderId;
            CustomerId = evt.CustomerId;
            Status = nameof(EOrderStatus.Pending);
        }

        /// <summary>
        /// Creates a snapshot of the Order's current state.
        /// </summary>
        public override IEventSourcedSnapshot CreateSnapshot()
        {
            return new OrderSnapshot
            {
                AggregateId = this.Id,
                Version = this.CurrentVersion,
                CustomerId = this.CustomerId,
                OrderType = this.OrderType,
                Status = this.Status,
                LineItems = new List<OrderLineItem>(this.LineItems)
            };
        }

        /// <summary>
        /// Restores the Order's state from a snapshot.
        /// </summary>
        public override void RestoreFromSnapshot(IEventSourcedSnapshot snapshot)
        {
            if (snapshot is not OrderSnapshot orderSnapshot) return;

            Id = orderSnapshot.AggregateId;
            CurrentVersion = orderSnapshot.Version;
            CustomerId = orderSnapshot.CustomerId;
            OrderType = orderSnapshot.OrderType;
            Status = orderSnapshot.Status;
            _lineItems.AddRange(orderSnapshot.LineItems);
        }
    }
}