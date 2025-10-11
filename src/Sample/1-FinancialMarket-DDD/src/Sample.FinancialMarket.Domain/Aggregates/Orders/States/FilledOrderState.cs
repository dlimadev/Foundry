using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    /// <summary>
    /// Represents the 'Filled' state. This is a final state; no further actions are permitted.
    /// Any attempt to transition from this state will result in an exception.
    /// </summary>
    public class FilledOrderState : BaseOrderState
    {
        public override EOrderStatus GetStatus() => EOrderStatus.Filled;
    }
}
