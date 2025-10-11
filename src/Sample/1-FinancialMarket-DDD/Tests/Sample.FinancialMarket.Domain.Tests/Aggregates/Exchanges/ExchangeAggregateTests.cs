using FluentAssertions;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;

namespace Sample.FinancialMarket.Domain.Tests.Aggregates.Exchanges
{
    /// <summary>
    /// Unit tests for the simple CRUD Aggregate, Exchange.
    /// </summary>
    public class ExchangeAggregateTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var name = "New York Stock Exchange";
            var acronym = "NYSE";
            var country = "USA";

            // Act
            var exchange = Exchange.Create(name, acronym, country);

            // Assert
            exchange.Name.Should().Be(name);
            exchange.Acronym.Should().Be(acronym);
            exchange.Country.Should().Be(country);
        }

        [Fact]
        public void UpdateDetails_ShouldChangeNameAndCountry_ButNotAcronym()
        {
            // Arrange
            var exchange = Exchange.Create("New York Stock Exchange", "NYSE", "USA");
            var newName = "NYSE American";
            var newCountry = "United States";

            // Act
            exchange.UpdateDetails(newName, newCountry);

            // Assert
            exchange.Name.Should().Be(newName);
            exchange.Country.Should().Be(newCountry);
            exchange.Acronym.Should().Be("NYSE"); 
        }
    }
}