namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios
{
    /// <summary>
    /// Represents a fixed-income asset, such as a bond.
    /// This is another 'Leaf' in the Composite pattern.
    /// </summary>
    public class Bond : FinancialAsset
    {
        public decimal InterestRate { get; private set; }
        public DateTime MaturityDate { get; private set; }
        public string IssuerName { get; private set; } = string.Empty;

        private Bond() { } // For EF Core

        public static Bond Create(string ticker, string issuerName, decimal price, decimal interestRate, DateTime maturityDate)
        {
            return new Bond
            {
                Id = Guid.NewGuid(),
                Ticker = ticker,
                IssuerName = issuerName,
                Price = price,
                InterestRate = interestRate,
                MaturityDate = maturityDate
            };
        }
    }
}