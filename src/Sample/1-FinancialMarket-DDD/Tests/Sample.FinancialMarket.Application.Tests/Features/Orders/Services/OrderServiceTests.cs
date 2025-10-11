// The code should be in English
using FluentAssertions;
using Foundry.Application.Abstractions.Mappings;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Moq;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Orders.Services;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using System.Net;

namespace Sample.FinancialMarket.Application.Tests.Features.Orders.Services
{
    /// <summary>
    /// Unit tests for the OrderService.
    /// These tests verify the service's orchestration logic by mocking its dependencies.
    /// </summary>
    [Trait("Category", "Unit")]
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly INotificationHandler _notificationHandler;
        private readonly Mock<IOrderValidationService> _mockValidationService;
        private readonly Mock<IMapper<Order, OrderDto>> _mockOrderMapper;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _notificationHandler = new NotificationHandler();
            _mockValidationService = new Mock<IOrderValidationService>();
            _mockOrderMapper = new Mock<IMapper<Order, OrderDto>>();

            _orderService = new OrderService(
                _mockUnitOfWork.Object,
                _mockOrderRepository.Object,
                _notificationHandler,
                _mockValidationService.Object,
                _mockOrderMapper.Object);
        }

        [Fact]
        public async Task OpenOrderAsync_WhenOrderExistsAndValidationSucceeds_ShouldReturnSuccess()
        {
            // --- Arrange ---
            var orderId = Guid.NewGuid();
            // We use a real domain object because its internal logic is tested elsewhere.
            // Here, we just need an object to pass around.
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));

            // Setup the repository mock to return our test order.
            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Setup the validation service mock to simulate a successful validation.
            _mockValidationService
                .Setup(v => v.ValidateForOpening(It.IsAny<Order>(), It.IsAny<INotificationHandler>()))
                .Returns(Task.CompletedTask);

            // --- Act ---
            var result = await _orderService.OpenOrderAsync(orderId);

            // --- Assert ---
            result.IsSuccess.Should().BeTrue();

            // Verify that the transaction was committed.
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OpenOrderAsync_WhenOrderNotFound_ShouldReturnFailureWithNotFoundStatus()
        {
            // --- Arrange ---
            var orderId = Guid.NewGuid();

            // Setup the repository mock to return null, simulating that the order does not exist.
            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

            // --- Act ---
            var result = await _orderService.OpenOrderAsync(orderId);

            // --- Assert ---
            result.IsSuccess.Should().BeFalse();
            result.SuggestedStatusCode.Should().Be(HttpStatusCode.NotFound);
            _notificationHandler.Notifications.Should().Contain(n => n.Key == "order.notFound");

            // Verify that the Unit of Work was NEVER called since the operation failed early.
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task OpenOrderAsync_WhenValidationChainFails_ShouldReturnFailureAndNotCommit()
        {
            // --- Arrange ---
            var orderId = Guid.NewGuid();
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));

            _mockOrderRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Setup the validation service mock to simulate a FAILED validation by adding an error.
            _mockValidationService
                .Setup(v => v.ValidateForOpening(It.IsAny<Order>(), It.IsAny<INotificationHandler>()))
                .Callback<Order, INotificationHandler>((o, n) => n.AddError("order.creditRisk.exceeded", "Credit check failed"));

            // --- Act ---
            var result = await _orderService.OpenOrderAsync(orderId);

            // --- Assert ---
            result.IsSuccess.Should().BeFalse();
            _notificationHandler.Notifications.Should().Contain(n => n.Key == "order.creditRisk.exceeded");

            // Verify that the transaction was NEVER committed.
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}