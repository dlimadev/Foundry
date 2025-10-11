using Foundry.Domain.Interfaces;
using Foundry.Infrastructure.Persistence.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Concrete repository for the Stock aggregate, using state-based persistence with EF Core.
    /// It inherits all its functionality from the framework's GenericRepository and implements
    /// the application-specific IStockRepository interface.
    /// </summary>
    public class StockRepository : GenericRepository<Stock>, IStockRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockRepository"/> class.
        /// </summary>
        /// <param name="context">The application's DbContext, provided via DI. It is of type IApplicationDbContext
        /// to enforce that SaveChanges is not called directly from the repository.</param>
        public StockRepository(IApplicationDbContext context) : base(context)
        {
            // No custom logic is needed here as all standard repository functionality
            // is provided by the base GenericRepository<Stock> from the framework.
            // If we needed a custom query method for Stocks, it would be implemented here.
        }
    }
}