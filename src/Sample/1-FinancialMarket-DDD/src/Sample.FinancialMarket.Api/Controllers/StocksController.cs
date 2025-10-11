// The code should be in English
using Asp.Versioning;
using Foundry.Api.BuildingBlocks.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Portfolios.Services;

namespace Sample.FinancialMarket.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing Stocks.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/stocks")]
    public class StocksController : BaseApiController
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        /// <summary>
        /// Creates a new stock.
        /// </summary>
        /// <remarks>
        /// Demonstrates Notification Pattern with FluentValidation and specific status code (409 Conflict) suggestion from the service.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(StockDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateStock([FromBody] CreateStockRequest request)
        {
            var result = await _stockService.CreateStockAsync(request);
            return HandleResult(result);
        }

        /// <summary>
        /// Retrieves a list of large-capitalization technology stocks.
        /// </summary>
        /// <remarks>
        /// Demonstrates the Specification pattern.
        /// </remarks>
        [HttpGet("queries/large-cap-tech")]
        public async Task<IActionResult> GetLargeCapTechStocks()
        {
            var result = await _stockService.GetLargeCapTechStocksAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// Gets a specific stock by its ID (Version 1.0).
        /// </summary>
        [HttpGet("{id:guid}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetStockByIdV1(Guid id)
        {
            var result = await _stockService.GetStockByIdV1Async(id);
            return HandleResult(result);
        }

        /// <summary>
        /// Gets a specific stock by its ID, with additional information (Version 2.0).
        /// </summary>
        [HttpGet("{id:guid}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetStockByIdV2(Guid id)
        {
            var result = await _stockService.GetStockByIdV2Async(id);
            return HandleResult(result);
        }
    }
}