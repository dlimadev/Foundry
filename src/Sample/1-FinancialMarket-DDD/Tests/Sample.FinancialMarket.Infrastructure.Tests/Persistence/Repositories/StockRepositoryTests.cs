using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios.Specifications;
using Sample.FinancialMarket.Infrastructure.Persistence;
using Sample.FinancialMarket.Infrastructure.Persistence.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Tests.Persistence.Repositories
{
    /// <summary>
    /// Integration tests for the repository layer.
    /// These tests validate the interaction between the repository and an in-memory database.
    /// </summary>
    public class StockRepositoryTests : IDisposable
    {
        private readonly FinanceDbContext _context;
        private readonly StockRepository _stockRepository;

        public StockRepositoryTests()
        {
            // --- Arrange: Setup ---
            // For each test, we create a fresh, isolated in-memory DbContext.
            var options = new DbContextOptionsBuilder<FinanceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FinanceDbContext(options);

            // Instantiate the repository with the real in-memory context.
            _stockRepository = new StockRepository(_context);
        }

        [Fact]
        public async Task ListAsync_WithSpecification_ShouldReturnCorrectlyFilteredAndOrderedData()
        {
            // --- Arrange: Seeding Data ---
            await _context.Stocks.AddRangeAsync(
                Stock.Create("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000m),
                Stock.Create("AAPL", "Apple", "Technology", 170, 2_500_000_000_000m),
                Stock.Create("GOOGL", "Alphabet", "Technology", 140, 1_800_000_000_000m),
                Stock.Create("TSLA", "Tesla", "Automotive", 250, 800_000_000_000m),
                Stock.Create("XOM", "Exxon", "Energy", 110, 400_000_000_000m)
            );
            await _context.SaveChangesAsync();

            var spec = new LargeCapTechStocksSpecification();

            // --- Act ---
            var result = await _stockRepository.ListAsync(spec);

            // --- Assert ---
            result.Should().HaveCount(3);
            result.First().Ticker.Should().Be("AAPL");
            result.Last().Ticker.Should().Be("GOOGL");
        }

        [Fact]
        public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
        {
            // --- Arrange ---
            var stock = Stock.Create("NVDA", "Nvidia", "Technology", 500, 1_200_000_000_000m);
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            // --- Act ---
            var result = await _stockRepository.GetByIdAsync(stock.Id);

            // --- Assert ---
            result.Should().NotBeNull();
            result!.Id.Should().Be(stock.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
        {
            // --- Act ---
            var result = await _stockRepository.GetByIdAsync(Guid.NewGuid());

            // --- Assert ---
            result.Should().BeNull();
        }

        // Helper to dispose of the in-memory database context after each test.
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}