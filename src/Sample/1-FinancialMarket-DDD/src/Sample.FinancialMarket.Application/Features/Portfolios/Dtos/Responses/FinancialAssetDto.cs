namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses
{
    /// <summary>
    /// A DTO to represent a financial asset in a portfolio's structure.
    /// It includes a list of children to support the Composite pattern structure.
    /// </summary>
    public record FinancialAssetDto(
        Guid Id,
        string Ticker,
        string AssetType, // "Stock", "Bond", "Portfolio"
        decimal Price,
        List<FinancialAssetDto> Children
    );
}