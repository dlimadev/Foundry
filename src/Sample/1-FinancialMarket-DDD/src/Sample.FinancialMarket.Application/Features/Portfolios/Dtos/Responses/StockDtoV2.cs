namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses
{
    public record StockDtoV2(
        Guid Id,
        string Ticker,
        string CompanyName,
        decimal Price,
        string AnalystRating
    );
}