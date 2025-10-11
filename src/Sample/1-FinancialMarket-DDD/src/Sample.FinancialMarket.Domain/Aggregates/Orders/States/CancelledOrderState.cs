using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    /// <summary>
    /// Represents the 'Cancelled' state. This is a final state; no further actions are permitted.
    /// </summary>
    public class CancelledOrderState : BaseOrderState
    {
        public override EOrderStatus GetStatus() => EOrderStatus.Cancelled;
    }
}
