namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests
{
    /// <summary>
    /// Data Transfer Object for a request to create a new Stock.
    /// It contains only the data required from the client to perform the creation.
    /// </summary>
    /// <param name="Ticker">The stock's ticker symbol (e.g., MSFT).</param>
    /// <param name="CompanyName">The full name of the company.</param>
    /// <param name="Sector">The industry sector.</param>
    /// <param name="Price">The current price of the stock.</param>
    /// <param name="MarketCap">The market capitalization of the company.</param>
    public record CreateStockRequest(
        string Ticker,
        string CompanyName,
        string Sector,
        decimal Price,
        decimal MarketCap
    );
}