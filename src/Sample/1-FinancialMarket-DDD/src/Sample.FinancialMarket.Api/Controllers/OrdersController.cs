using Asp.Versioning;
using Foundry.Api.BuildingBlocks.Controllers;
using Microsoft.AspNetCore.Mvc;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Orders.Services;

namespace Sample.FinancialMarket.Api.Controllers
{
    /// <summary>
    /// API endpoints for managing the Order lifecycle.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new Order in a 'Pending' state.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            return HandleResult(result);
        }
        /// <summary>
        /// Adds a new line item to an existing order.
        /// </summary>
        [HttpPost("{id:guid}/items")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddOrderItemRequest request)
        {
            var result = await _orderService.AddItemToOrderAsync(id, request);
            return HandleResult(result);
        }

        /// <summary>
        /// Attempts to open an order after running it through a validation chain.
        /// </summary>
        /// <remarks>
        /// Demonstrates the Chain of Responsibility and State patterns.
        /// </remarks>
        [HttpPost("{id:guid}/open")]
        public async Task<IActionResult> OpenOrder(Guid id)
        {
            var result = await _orderService.OpenOrderAsync(id);
            return HandleResult(result);
        }
    }
}