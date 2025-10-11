namespace Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests
{
    /// <summary>
    /// DTO for a request to create a new, empty Portfolio.
    /// </summary>
    /// <param name="Ticker">The unique ticker for the new portfolio.</param>
    /// <param name="Name">The user-friendly name for the portfolio.</param>
    /// <param name="Description">An optional description for the portfolio.</param>
    public record CreatePortfolioRequest(
        string Ticker,
        string Name,
        string? Description
    );
}