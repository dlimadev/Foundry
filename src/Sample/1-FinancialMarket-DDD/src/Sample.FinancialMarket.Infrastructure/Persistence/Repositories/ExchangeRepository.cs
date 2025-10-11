using Foundry.Domain.Interfaces;
using Foundry.Infrastructure.Persistence.Repositories;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Repositories
{
    public class ExchangeRepository : GenericRepository<Exchange>, IExchangeRepository
    {
        public ExchangeRepository(IApplicationDbContext context) : base(context) { }
    }
}