using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Aggregates.Exchanges
{
    /// <summary>
    /// Represents a stock exchange (e.g., NASDAQ, NYSE).
    /// This is a simple Aggregate Root, ideal for being managed by a GenericCrudService.
    /// </summary>
    public class Exchange : EntityBase, IAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Acronym { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;

        private Exchange() { } // For EF Core

        /// <summary>
        /// Factory method to create a new Exchange, ensuring it's in a valid state.
        /// </summary>
        public static Exchange Create(string name, string acronym, string country)
        {
            // Validation will be handled by FluentValidation, but we still ensure construction.
            return new Exchange
            {
                Id = Guid.NewGuid(),
                Name = name,
                Acronym = acronym,
                Country = country
            };
        }

        /// <summary>
        /// Business method to update the exchange's details.
        /// </summary>
        public void UpdateDetails(string newName, string newCountry)
        {
            // Even in a simple CRUD entity, we centralize the update logic in a business method.
            Name = newName;
            Country = newCountry;
        }
    }
}