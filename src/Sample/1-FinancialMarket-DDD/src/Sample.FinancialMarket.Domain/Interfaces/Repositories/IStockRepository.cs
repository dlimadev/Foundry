using Foundry.Domain.Interfaces.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Defines the contract for a repository that manages Stock entities.
    /// It inherits all the standard data access methods from the framework's IGenericRepository<Stock>.
    /// </summary>
    public interface IStockRepository : IGenericRepository<Stock>
    {
        // This is the place to add custom query methods specific to Stocks in the future.
        // For example:
        // Task<Stock?> GetByIsinCodeAsync(string isinCode);
    }
}