namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses
{
    public record StockDto(
        Guid Id,
        string Ticker,
        string CompanyName,
        string Sector,
        decimal Price,
        decimal MarketCap
    );
}