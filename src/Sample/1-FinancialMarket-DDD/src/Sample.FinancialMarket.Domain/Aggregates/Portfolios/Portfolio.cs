using Foundry.Domain.Model;
using Foundry.Domain.Exceptions;
using Foundry.Domain.Model.Attributes;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios
{
    /// <summary>
    /// Represents a collection of other financial assets.
    /// This is the 'Composite' in the Composite Pattern and an Aggregate Root.
    /// </summary>
    [Cacheable(15)]
    public class Portfolio : FinancialAsset, IAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        private readonly List<FinancialAsset> _assets = new();
        public IReadOnlyCollection<FinancialAsset> Assets => _assets.AsReadOnly();

        private Portfolio() { } // For EF Core

        public static Portfolio Create(string ticker, string name, string? description = null)
        {
            return new Portfolio
            {
                Id = Guid.NewGuid(),
                Ticker = ticker,
                Name = name,
                Description = description,
                Price = 0
            };
        }

        public override void Add(FinancialAsset asset)
        {
            if (asset.Id != Guid.Empty && asset.Id == Id)
                throw new DomainException("portfolio.cannotContainItself");

            _assets.Add(asset);
        }

        public override void Remove(FinancialAsset asset) => _assets.Remove(asset);

        public void UpdateDetails(string newName, string? newDescription)
        {
            Name = newName;
            Description = newDescription;
        }
    }
}