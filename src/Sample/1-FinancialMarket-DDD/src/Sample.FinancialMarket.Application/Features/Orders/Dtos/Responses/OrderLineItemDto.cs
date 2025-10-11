namespace Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses
{
    /// <summary>
    /// Data Transfer Object for an order line item.
    /// </summary>
    public record OrderLineItemDto(
        Guid Id,
        string Ticker,
        int Quantity,
        decimal Price,
        string Currency
    );
}
