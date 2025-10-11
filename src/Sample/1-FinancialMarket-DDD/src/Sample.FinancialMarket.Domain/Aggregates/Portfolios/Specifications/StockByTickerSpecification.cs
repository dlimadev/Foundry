using Foundry.Domain.Interfaces.Specifications;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios.Specifications
{
    public class StockByTickerSpecification : BaseSpecification<Stock>
    {
        public StockByTickerSpecification(string ticker)
            : base(s => s.Ticker.ToUpper() == ticker.ToUpper())
        {
        }
    }
}