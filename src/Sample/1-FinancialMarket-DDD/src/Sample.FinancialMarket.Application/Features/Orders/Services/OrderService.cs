// The code should be in English
using Foundry.Application.Abstractions.Mappings;
using Foundry.Application.Abstractions.Responses;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;
using Sample.FinancialMarket.Domain.Common.ValueObjects;
using Sample.FinancialMarket.Domain.Interfaces;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sample.FinancialMarket.Application.Features.Orders.Services
{
    /// <summary>
    /// Implements the use cases for the Order aggregate.
    /// It orchestrates domain entities and infrastructure services to fulfill business requirements.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationHandler _notifier;
        private readonly IOrderValidationService _validationService;
        private readonly IMapper<Order, OrderDto> _orderMapper;

        public OrderService(
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            INotificationHandler notifier,
            IOrderValidationService validationService,
            IMapper<Order, OrderDto> orderMapper)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _notifier = notifier;
            _validationService = validationService;
            _orderMapper = orderMapper;
        }

        /// <inheritdoc />
        public async Task<Result<OrderDto>> CreateOrderAsync(CreateOrderRequest request)
        {
            if (!Enum.TryParse<EOrderType>(request.OrderType, true, out var eOrderType))
            {
                _notifier.AddError("order.invalidType", "The provided order type is invalid.");
                return Result<OrderDto>.Failure(_notifier.Notifications);
            }

            var order = Order.Create(request.CustomerId, eOrderType, request.ExpirationDate);

            await _orderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return Result<OrderDto>.Success(_orderMapper.Map(order), HttpStatusCode.Created);
        }

        /// <inheritdoc />
        public async Task<Result<OrderDto>> AddItemToOrderAsync(Guid orderId, AddOrderItemRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _notifier.AddError("order.notFound", $"Order with ID '{orderId}' was not found.");
                return Result<OrderDto>.Failure(_notifier.Notifications, HttpStatusCode.NotFound);
            }

            var price = Money.Create(request.Amount, request.Currency);

            // The business logic and its invariants are protected inside the domain entity.
            // A DomainException will be thrown if the rule is violated, caught by the global handler.
            order.AddItem(request.Ticker, request.Quantity, price);

            _orderRepository.Update(order);
            await _unitOfWork.CompleteAsync();

            return Result<OrderDto>.Success(_orderMapper.Map(order));
        }

        /// <inheritdoc />
        public async Task<Result<OrderDto>> OpenOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _notifier.AddError("order.notFound", $"Order with ID '{orderId}' was not found.");
                return Result<OrderDto>.Failure(_notifier.Notifications, HttpStatusCode.NotFound);
            }

            // "Tell, Don't Ask": Tell the entity to open, providing the validation service it needs.
            await order.Open(_validationService, _notifier);

            if (_notifier.HasErrors)
            {
                // Errors were collected by the Chain of Responsibility inside the validation service.
                return Result<OrderDto>.Failure(_notifier.Notifications);
            }

            await _unitOfWork.CompleteAsync();
            return Result<OrderDto>.Success(_orderMapper.Map(order));
        }
    }
}