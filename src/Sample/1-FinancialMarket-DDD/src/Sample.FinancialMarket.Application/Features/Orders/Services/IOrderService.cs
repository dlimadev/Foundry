using Foundry.Application.Abstractions.Responses;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses;

namespace Sample.FinancialMarket.Application.Features.Orders.Services
{
    /// <summary>
    /// Defines the contract for the Order use case orchestrator.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Use case to create a new, empty order.
        /// </summary>
        Task<Result<OrderDto>> CreateOrderAsync(CreateOrderRequest request);

        /// <summary>
        /// Use case to add a new line item to an existing order.
        /// </summary>
        Task<Result<OrderDto>> AddItemToOrderAsync(Guid orderId, AddOrderItemRequest request);

        /// <summary>
        /// Use case to validate and open an existing order for trading.
        /// </summary>
        Task<Result<OrderDto>> OpenOrderAsync(Guid orderId);
    }
}