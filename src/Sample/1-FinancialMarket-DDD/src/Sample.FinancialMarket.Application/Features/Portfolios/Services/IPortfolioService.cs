using Foundry.Application.Abstractions.Responses;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Services
{
    /// <summary>
    /// Defines the contract for use cases related to the Portfolio aggregate.
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        /// Creates a new, empty portfolio.
        /// </summary>
        Task<Result<PortfolioDto>> CreatePortfolioAsync(CreatePortfolioRequest request);

        /// <summary>
        /// Gets the hierarchical structure of a portfolio for display, demonstrating the Composite pattern.
        /// </summary>
        Task<Result<FinancialAssetDto>> GetPortfolioStructureAsync(Guid portfolioId);
    }
}