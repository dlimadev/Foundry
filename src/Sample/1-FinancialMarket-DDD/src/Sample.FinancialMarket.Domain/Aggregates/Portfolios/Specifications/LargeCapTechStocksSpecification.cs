// The code should be in English
using Foundry.Domain.Interfaces.Specifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios.Specifications
{
    /// <summary>
    /// A concrete specification to find all large-capitalization technology stocks.
    /// This encapsulates a specific business query in a reusable object.
    /// </summary>
    public class LargeCapTechStocksSpecification : BaseSpecification<Stock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeCapTechStocksSpecification"/> class.
        /// </summary>
        public LargeCapTechStocksSpecification()
            // The criteria defines the 'Where' clause of the query.
            : base(stock => stock.Sector == "Technology" && stock.MarketCap > 1_000_000_000_000m)
        {
            // We can also apply ordering.
            ApplyOrderByDescending(stock => stock.MarketCap);
        }
    }
}