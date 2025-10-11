using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    /// <summary>
    /// An abstract base class to reduce code duplication among concrete state classes.
    /// It provides a default implementation for all actions that throws an exception,
    /// so concrete states only need to override the actions that are valid for them.
    /// </summary>
    public abstract class BaseOrderState : IOrderState
    {
        public virtual void Open(Order order) => ThrowInvalidTransition();
        public virtual void Fill(Order order) => ThrowInvalidTransition();
        public virtual void Cancel(Order order) => ThrowInvalidTransition();
        public abstract EOrderStatus GetStatus();
        protected void ThrowInvalidTransition() => throw new InvalidOperationException($"Cannot perform this action in the current state: {GetStatus()}");
    }
}
