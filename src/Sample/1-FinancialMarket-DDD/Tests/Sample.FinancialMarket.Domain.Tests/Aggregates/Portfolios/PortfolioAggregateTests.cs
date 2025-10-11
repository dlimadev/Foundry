// The code should be in English
using FluentAssertions;
using Foundry.Domain.Exceptions;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Domain.Tests.Aggregates.Portfolios
{
    /// <summary>
    /// Unit tests for the Portfolio Aggregate Root and its interaction with other FinancialAsset entities.
    /// These tests focus on the implementation of the Composite pattern.
    /// </summary>
    public class PortfolioAggregateTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateEmptyPortfolio()
        {
            // Arrange
            var ticker = "MAIN_PF";
            var name = "Main Portfolio";

            // Act
            var portfolio = Portfolio.Create(ticker, name);

            // Assert
            portfolio.Should().NotBeNull();
            portfolio.Ticker.Should().Be(ticker);
            portfolio.Name.Should().Be(name);
            portfolio.Assets.Should().BeEmpty();
        }

        [Fact]
        public void Add_LeafAsset_ShouldAddAssetToCollection()
        {
            // Arrange
            var portfolio = Portfolio.Create("MAIN_PF", "Main Portfolio");
            var stock = Stock.Create("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000);

            // Act
            portfolio.Add(stock);

            // Assert
            portfolio.Assets.Should().HaveCount(1);
            portfolio.Assets.Should().Contain(stock);
        }

        [Fact]
        public void Add_CompositeAsset_ShouldAddSubPortfolioToCollection()
        {
            // Arrange
            var mainPortfolio = Portfolio.Create("MAIN_PF", "Main Portfolio");
            var techPortfolio = Portfolio.Create("TECH_PF", "Tech Portfolio");

            // Act
            mainPortfolio.Add(techPortfolio);

            // Assert
            mainPortfolio.Assets.Should().HaveCount(1);
            mainPortfolio.Assets.Should().Contain(techPortfolio);
        }

        [Fact]
        public void Add_Itself_ShouldThrowDomainException()
        {
            // Arrange
            var portfolio = Portfolio.Create("MAIN_PF", "Main Portfolio");
            // We need to simulate the Id being set by the database for this check to work.
            // Using Reflection is a common technique in unit tests to set private/protected properties.
            var idProperty = typeof(Foundry.Domain.Model.EntityBase).GetProperty("Id");
            idProperty.SetValue(portfolio, Guid.NewGuid());

            // Act
            Action act = () => portfolio.Add(portfolio);

            // Assert
            // This tests the crucial invariant that a portfolio cannot contain itself.
            act.Should().Throw<DomainException>()
                .Where(ex => ex.ErrorCode == "portfolio.cannotContainItself");
        }

        [Fact]
        public void Remove_ExistingAsset_ShouldRemoveAssetFromCollection()
        {
            // Arrange
            var portfolio = Portfolio.Create("MAIN_PF", "Main Portfolio");
            var stock = Stock.Create("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000);
            portfolio.Add(stock);

            // Act
            portfolio.Remove(stock);

            // Assert
            portfolio.Assets.Should().BeEmpty();
        }

        [Fact]
        public void Add_ToLeafAsset_ShouldThrowNotSupportedException()
        {
            // Arrange
            var stock = Stock.Create("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000);
            var otherStock = Stock.Create("AAPL", "Apple", "Technology", 170, 2_500_000_000_000);

            // Act
            Action act = () => stock.Add(otherStock);

            // Assert
            // This tests that the 'Leaf' objects correctly prevent children from being added.
            act.Should().Throw<NotSupportedException>();
        }
    }
}