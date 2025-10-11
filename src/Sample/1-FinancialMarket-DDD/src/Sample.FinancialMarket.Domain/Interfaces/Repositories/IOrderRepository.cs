using Foundry.Domain.Interfaces.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Orders;

namespace Sample.FinancialMarket.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order> { }
}