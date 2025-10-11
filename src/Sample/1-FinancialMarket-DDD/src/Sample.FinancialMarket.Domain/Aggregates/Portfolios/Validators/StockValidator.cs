using FluentValidation;

namespace Sample.FinancialMarket.Domain.Aggregates.Portfolios.Validators
{
    public class StockValidator : AbstractValidator<Stock>
    {
        public StockValidator()
        {
            RuleFor(stock => stock.Ticker)
                .NotEmpty().WithErrorCode("stock.ticker.required");

            RuleFor(stock => stock.CompanyName)
                .NotEmpty().WithErrorCode("stock.companyName.required")
                .MaximumLength(100).WithErrorCode("stock.companyName.maxLength");

            RuleFor(stock => stock.Price)
                .GreaterThan(0).WithErrorCode("stock.price.invalid");
        }
    }
}