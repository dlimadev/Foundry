namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses
{
    /// <summary>
    /// DTO for returning Portfolio details.
    /// </summary>
    public record PortfolioDto(
        Guid Id,
        string Ticker,
        string Name,
        string? Description,
        int AssetCount
    );
}