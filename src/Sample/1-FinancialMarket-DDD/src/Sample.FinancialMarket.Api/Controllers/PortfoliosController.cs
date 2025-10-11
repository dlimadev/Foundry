using Asp.Versioning;
using Foundry.Api.BuildingBlocks.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Services;

namespace Sample.FinancialMarket.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing Portfolios.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/portfolios")]
    public class PortfoliosController : BaseApiController
    {
        private readonly IPortfolioService _portfolioService;

        public PortfoliosController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        /// <summary>
        /// Creates a new, empty portfolio.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioRequest request)
        {
            var result = await _portfolioService.CreatePortfolioAsync(request);

            return HandleResult(result);
        }

        /// <summary>
        /// Gets the hierarchical structure of a portfolio and its total value.
        /// </summary>
        /// <remarks>
        /// Demonstrates the Composite pattern.
        /// </remarks>
        [HttpGet("{id:guid}/structure")]
        public async Task<IActionResult> GetPortfolioStructure(Guid id)
        {
            var result = await _portfolioService.GetPortfolioStructureAsync(id);
            return HandleResult(result);
        }
    }
}