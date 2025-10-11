using Foundry.Domain.Interfaces.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Specific contract for the Portfolio repository.
    /// It inherits the common CRUD operations from IGenericRepository and can
    /// add query methods that are specific to Portfolios.
    /// </summary>
    public interface IPortfolioRepository : IGenericRepository<Portfolio>
    {
        Task<Portfolio?> GetByTickerWithAssetsAsync(string ticker);
    }
}