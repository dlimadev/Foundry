using Foundry.Domain.Interfaces.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;

namespace Sample.FinancialMarket.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contract for the Exchange repository.
    /// </summary>
    public interface IExchangeRepository : IGenericRepository<Exchange>
    {
    }
}