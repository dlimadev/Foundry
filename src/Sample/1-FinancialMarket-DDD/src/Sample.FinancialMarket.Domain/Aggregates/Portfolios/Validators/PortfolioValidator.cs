using FluentValidation;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios.Validators
{
    public class PortfolioValidator : AbstractValidator<Portfolio>
    {
        public PortfolioValidator()
        {
            RuleFor(portfolio => portfolio.Ticker)
                .NotEmpty().WithErrorCode("portfolio.ticker.required")
                .MaximumLength(20).WithErrorCode("portfolio.ticker.maxLength");

            RuleFor(portfolio => portfolio.Name)
                .NotEmpty().WithErrorCode("portfolio.name.required")
                .MaximumLength(100).WithErrorCode("portfolio.name.maxLength");

            RuleFor(portfolio => portfolio.Description)
                .MaximumLength(500).WithErrorCode("portfolio.description.maxLength");
        }
    }
}