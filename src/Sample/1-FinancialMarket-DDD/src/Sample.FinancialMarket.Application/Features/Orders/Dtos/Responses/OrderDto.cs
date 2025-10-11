namespace Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses
{
    /// <summary>
    /// Data Transfer Object for returning Order details through the API.
    /// </summary>
    public record OrderDto(
        Guid Id,
        string CustomerId,
        string Status,
        string OrderType,
        decimal TotalValue,
        string Currency,
        DateTime ExpirationDate,
        IReadOnlyCollection<OrderLineItemDto> LineItems
    );
}