
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Infrastructure.Persistence;
using Sample.FinancialMarket.Infrastructure.Persistence.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Tests.Persistence.Repositories
{
    [Trait("Category", "Integration")]
    public class PortfolioRepositoryTests : IDisposable
    {
        private readonly FinanceDbContext _context;
        private readonly PortfolioRepository _portfolioRepository;

        public PortfolioRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FinanceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // --- CORRECTION ---
            // The FinanceDbContext constructor is now simpler and no longer requires audit dependencies.
            // This aligns the test with our recent refactoring.
            _context = new FinanceDbContext(options);

            _portfolioRepository = new PortfolioRepository(_context);
        }

        [Fact]
        public async Task GetByIdWithAssetsRecursiveAsync_WhenPortfolioIsNested_ShouldLoadFullHierarchy()
        {
            // --- Arrange ---
            var stockMsft = Stock.Create("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000m);
            var stockAapl = Stock.Create("AAPL", "Apple", "Technology", 170, 2_500_000_000_000m);
            var techPortfolio = Portfolio.Create("TECH_PF", "Technology Portfolio");
            techPortfolio.Add(stockMsft);
            techPortfolio.Add(stockAapl);

            var mainPortfolio = Portfolio.Create("MAIN_PF", "Main Diversified Portfolio");
            mainPortfolio.Add(techPortfolio);

            await _context.Portfolios.AddAsync(mainPortfolio);
            await _context.SaveChangesAsync();

            // --- Act ---
            var result = await _portfolioRepository.GetByIdAsync(mainPortfolio.Id);

            // --- Assert ---
            result.Should().NotBeNull();
            var nestedPortfolio = result!.Assets.OfType<Portfolio>().FirstOrDefault();
            nestedPortfolio.Should().NotBeNull();
            nestedPortfolio!.Assets.Should().HaveCount(2);
        }

        public void Dispose() => _context.Dispose();
    }
}