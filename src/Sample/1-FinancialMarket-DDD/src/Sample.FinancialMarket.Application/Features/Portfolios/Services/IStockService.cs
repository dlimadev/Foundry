// The code should be in English
using Foundry.Application.Abstractions.Responses;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Services
{
    /// <summary>
    /// Defines the contract for use cases related to the Stock aggregate.
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Creates a new stock after performing validation and business rule checks.
        /// </summary>
        Task<Result<StockDto>> CreateStockAsync(CreateStockRequest request);

        /// <summary>
        /// Gets a list of stocks that match the large-cap technology criteria.
        /// </summary>
        Task<Result<IReadOnlyList<StockDto>>> GetLargeCapTechStocksAsync();

        /// <summary>
        /// Gets a specific stock by its ID, formatted for API V1.
        /// </summary>
        Task<Result<StockDto>> GetStockByIdV1Async(Guid id);

        /// <summary>
        /// Gets a specific stock by its ID, formatted for API V2.
        /// </summary>
        Task<Result<StockDtoV2>> GetStockByIdV2Async(Guid id);
    }
}