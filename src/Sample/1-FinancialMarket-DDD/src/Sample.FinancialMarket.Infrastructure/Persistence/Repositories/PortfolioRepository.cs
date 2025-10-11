using Foundry.Domain.Interfaces;
using Foundry.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Concrete repository for the Portfolio aggregate, using state-based persistence with EF Core.
    /// </summary>
    public class PortfolioRepository : GenericRepository<Portfolio>, IPortfolioRepository
    {
        public PortfolioRepository(IApplicationDbContext context) : base(context) { }

        /// <summary>
        /// A specific method to load a portfolio by its ticker and all its nested assets recursively.
        /// This method fulfills the IPortfolioRepository contract.
        /// </summary>
        public async Task<Portfolio?> GetByTickerWithAssetsAsync(string ticker)
        {
            if (_context is not DbContext concreteContext)
            {
                // This fallback is not ideal as it won't load nested assets.
                // In a real app, you might throw an exception if the context is not the expected type.
                return (await ListAllAsync()).FirstOrDefault(p => p.Ticker == ticker);
            }

            // The query now filters by Ticker instead of Id.
            return await concreteContext.Set<Portfolio>()
                .Include(p => p.Assets)
                    .ThenInclude(child => (child as Portfolio)!.Assets)
                .FirstOrDefaultAsync(p => p.Ticker == ticker);
        }
    }
}