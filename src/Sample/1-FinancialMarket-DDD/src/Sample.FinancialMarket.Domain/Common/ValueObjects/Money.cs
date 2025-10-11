using Foundry.Domain.Model;

namespace Sample.FinancialMarket.Domain.Common.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative.", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency must be specified.", nameof(currency));
            
            return new Money { Amount = amount, Currency = currency.ToUpper() };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}