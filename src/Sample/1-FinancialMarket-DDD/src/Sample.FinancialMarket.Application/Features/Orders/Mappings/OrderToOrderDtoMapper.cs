using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Orders;

namespace Sample.FinancialMarket.Application.Features.Orders.Mappings
{
    public class OrderToOrderDtoMapper : IMapper<Order, OrderDto>
    {
        public OrderDto Map(Order source)
        {
            if (source == null) return null!;

            var lineItemDtos = source.LineItems
                .Select(li => new OrderLineItemDto(li.Id, li.Ticker, li.Quantity, li.Price.Amount, li.Price.Currency))
                .ToList().AsReadOnly();

            return new OrderDto(
                source.Id,
                source.CustomerId,
                source.Status,
                source.OrderType.ToString(),
                source.TotalValue.Amount,
                source.TotalValue.Currency,
                source.ExpirationDate,
                lineItemDtos
            );
        }
    }
}