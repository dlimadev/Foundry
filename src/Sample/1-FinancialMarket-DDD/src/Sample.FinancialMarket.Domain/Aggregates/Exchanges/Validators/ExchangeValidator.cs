using FluentValidation;

namespace Sample.FinancialMarket.Domain.Aggregates.Exchanges.Validators
{
    public class ExchangeValidator : AbstractValidator<Exchange>
    {
        public ExchangeValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithErrorCode("exchange.name.required").MaximumLength(100);
            RuleFor(e => e.Acronym).NotEmpty().WithErrorCode("exchange.acronym.required").MaximumLength(10);
            RuleFor(e => e.Country).NotEmpty().WithErrorCode("exchange.country.required").MaximumLength(50);
        }
    }
}