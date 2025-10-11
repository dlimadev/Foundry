using Foundry.Domain.Exceptions;
using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios
{
    /// <summary>
    /// Represents a single stock asset. It acts as a 'Leaf' in the Composite pattern
    /// and is also an Aggregate Root itself.
    /// </summary>
    public class Stock : FinancialAsset, IAggregateRoot
    {
        public string CompanyName { get; private set; } = string.Empty;
        public string Sector { get; private set; } = string.Empty;
        public decimal MarketCap { get; private set; }

        private Stock() { } // For EF Core

        public static Stock Create(string ticker, string companyName, string sector, decimal price, decimal marketCap)
        {
            // Validation is handled by FluentValidation in the Application Layer.
            // This factory method is responsible for object construction.
            return new Stock
            {
                Id = Guid.NewGuid(),
                Ticker = ticker,
                CompanyName = companyName,
                Sector = sector,
                Price = price,
                MarketCap = marketCap
            };
        }

        public void UpdateMarketData(decimal newPrice, decimal newMarketCap)
        {
            // Business Rule (Invariant)
            if (newPrice < 0)
                throw new DomainException("stock.price.negative", newPrice);

            Price = newPrice;
            MarketCap = newMarketCap;
        }
    }
}