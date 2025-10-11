using FluentValidation;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios.Validators
{
    public class BondValidator : AbstractValidator<Bond>
    {
        public BondValidator()
        {
            RuleFor(bond => bond.Ticker)
                .NotEmpty().WithErrorCode("bond.ticker.required");

            RuleFor(bond => bond.IssuerName)
                .NotEmpty().WithErrorCode("bond.issuerName.required");

            RuleFor(bond => bond.Price)
                .GreaterThan(0).WithErrorCode("bond.price.invalid");

            RuleFor(bond => bond.InterestRate)
                .GreaterThanOrEqualTo(0).WithErrorCode("bond.interestRate.invalid");

            RuleFor(bond => bond.MaturityDate)
                .GreaterThan(DateTime.UtcNow).WithErrorCode("bond.maturityDate.past");
        }
    }
}