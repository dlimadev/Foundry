using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.States
{
    public interface IOrderState
    {
        void Open(Order order);
        void Fill(Order order);
        void Cancel(Order order);
        EOrderStatus GetStatus();
    }
}