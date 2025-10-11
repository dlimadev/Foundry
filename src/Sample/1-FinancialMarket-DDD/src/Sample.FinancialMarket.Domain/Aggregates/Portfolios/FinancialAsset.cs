// The code should be in English
using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios
{
    /// <summary>
    /// Represents the abstract base class ('Component') for any financial asset in the system.
    /// It allows clients to treat individual assets (leaves) and compositions (composites) uniformly.
    /// </summary>
    public abstract class FinancialAsset : EntityBase
    {
        /// <summary>
        /// The unique ticker symbol for the asset (e.g., "MSFT", "PETR4").
        /// </summary>
        public string Ticker { get; protected set; } = string.Empty;

        /// <summary>
        /// The current market price of the asset.
        /// </summary>
        public decimal Price { get; protected set; }

        /// <summary>
        /// Part of the Composite Pattern. Base implementation throws an exception because
        /// 'Leaf' objects (like Stock) cannot have children.
        /// </summary>
        public virtual void Add(FinancialAsset asset) => throw new NotSupportedException("This asset type cannot contain other assets.");

        /// <summary>
        /// Part of the Composite Pattern. Base implementation.
        /// </summary>
        public virtual void Remove(FinancialAsset asset) => throw new NotSupportedException("This asset type cannot contain other assets.");
    }
}