namespace Sample.FinancialMarket.Application.Features.Orders.Dtos.Requests
{
    /// <summary>
    /// DTO for a request to add a new line item to an existing Order.
    /// </summary>
    /// <param name="Ticker">The ticker of the asset to be traded.</param>
    /// <param name="Quantity">The number of units to trade.</param>
    /// <param name="Amount">The price per unit.</param>
    /// <param name="Currency">The currency of the price (e.g., "USD", "EUR").</param>
    public record AddOrderItemRequest(
        string Ticker,
        int Quantity,
        decimal Amount,
        string Currency
    );
}