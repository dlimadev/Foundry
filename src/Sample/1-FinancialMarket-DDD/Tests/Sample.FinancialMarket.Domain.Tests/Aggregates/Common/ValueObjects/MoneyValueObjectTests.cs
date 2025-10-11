// The code should be in English
using FluentAssertions;
using Sample.FinancialMarket.Domain.Common.ValueObjects;

namespace Sample.FinancialMarket.Domain.Tests.Aggregates.Common.ValueObjects
{
    /// <summary>
    /// Unit tests for the Money Value Object.
    /// These tests focus on creation validation and, most importantly, value-based equality.
    /// </summary>
    public class MoneyValueObjectTests
    {
        [Fact]
        public void Create_WithNegativeAmount_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => Money.Create(-100, "USD");

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithInvalidCurrency_ShouldThrowArgumentException(string currency)
        {
            // Act
            Action act = () => Money.Create(100, currency);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Equality_WhenAmountAndCurrencyAreSame_ShouldBeEqual()
        {
            // Arrange
            var moneyA = Money.Create(150.75m, "EUR");
            var moneyB = Money.Create(150.75m, "EUR");

            // Assert
            // Test all forms of equality
            (moneyA == moneyB).Should().BeTrue();
            moneyA.Equals(moneyB).Should().BeTrue();
        }

        [Fact]
        public void Equality_WhenAmountsAreDifferent_ShouldNotBeEqual()
        {
            // Arrange
            var moneyA = Money.Create(150.75m, "EUR");
            var moneyB = Money.Create(150.76m, "EUR");

            // Assert
            (moneyA == moneyB).Should().BeFalse();
            moneyA.Equals(moneyB).Should().BeFalse();
            (moneyA != moneyB).Should().BeTrue();
        }

        [Fact]
        public void Equality_WhenCurrenciesAreDifferent_ShouldNotBeEqual()
        {
            // Arrange
            var moneyA = Money.Create(150.75m, "EUR");
            var moneyB = Money.Create(150.75m, "USD");

            // Assert
            (moneyA == moneyB).Should().BeFalse();
            moneyA.Equals(moneyB).Should().BeFalse();
        }

        [Fact]
        public void Equality_WhenComparedToNull_ShouldNotBeEqual()
        {
            // Arrange
            var moneyA = Money.Create(150.75m, "EUR");

            // Assert
            (moneyA == null).Should().BeFalse();
            moneyA.Equals(null).Should().BeFalse();
        }
    }
}