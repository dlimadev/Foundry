using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    /// <summary>
    /// Represents the 'Open' state. An order in this state can be filled (executed) or cancelled.
    /// </summary>
    public class OpenOrderState : BaseOrderState
    {
        public override EOrderStatus GetStatus() => EOrderStatus.Open;

        // A valid transition from Open is to Filled.
        public override void Fill(Order order) => order.ChangeState(new FilledOrderState());

        // A valid transition from Open is to Cancel.
        public override void Cancel(Order order) => order.ChangeState(new CancelledOrderState());
    }
}
