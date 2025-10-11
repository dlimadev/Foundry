namespace Sample.FinancialMarket.Application.Features.Orders.Dtos.Requests
{
    /// <summary>
    /// DTO for a request to create a new, empty Order.
    /// </summary>
    /// <param name="CustomerId">The ID of the customer placing the order.</param>
    /// <param name="OrderType">The type of the order ('Buy' or 'Sell').</param>
    /// <param name="ExpirationDate">The UTC date and time when the order will expire if not filled.</param>
    public record CreateOrderRequest(
        string CustomerId,
        string OrderType,
        DateTime ExpirationDate
    );
}