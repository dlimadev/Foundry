using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    /// <summary>
    /// Represents the 'Pending' state. An order in this state can be opened or cancelled.
    /// It is the initial state of a new order.
    /// </summary>
    public class PendingOrderState : BaseOrderState
    {
        public override EOrderStatus GetStatus() => EOrderStatus.Pending;

        // A valid transition from Pending is to Open.
        public override void Open(Order order) => order.ChangeState(new OpenOrderState());

        // A valid transition from Pending is to Cancel.
        public override void Cancel(Order order) => order.ChangeState(new CancelledOrderState());
    }
}
