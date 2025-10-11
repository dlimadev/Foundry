using FluentValidation;

namespace Sample.FinancialMarket.Domain.Aggregates.Orders.Validators
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(order => order.CustomerId)
                .NotEmpty().WithErrorCode("order.customerId.required");

            RuleFor(order => order.ExpirationDate)
                .GreaterThan(DateTime.UtcNow).WithErrorCode("order.expirationDate.past");

            RuleFor(order => order.LineItems)
                .NotEmpty().WithErrorCode("order.lineItems.empty");
        }
    }
}