// The code should be in English
using FluentAssertions;
using Foundry.Domain.Exceptions;
using Foundry.Domain.Notifications;
using Moq;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Enums;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Events;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;
using Sample.FinancialMarket.Domain.Aggregates.Orders.States;
using Sample.FinancialMarket.Domain.Common.ValueObjects;

namespace Sample.FinancialMarket.Domain.Tests.Aggregates.Orders
{
    /// <summary>
    /// Unit tests for the Order Aggregate Root.
    /// These tests focus on the business logic and invariants of the Order entity in isolation.
    /// </summary>
    public class OrderAggregateTests
    {
        // --- Testes de Criação e Estado Inicial ---

        [Fact]
        public void Create_WithValidParameters_ShouldCreateOrderInPendingState_And_RaiseOrderCreatedEvent()
        {
            // Arrange
            var customerId = "customer-123";
            var expirationDate = DateTime.UtcNow.AddDays(1);

            // Act
            var order = Order.Create(customerId, EOrderType.Buy, expirationDate);

            // Assert
            order.Should().NotBeNull();
            order.CustomerId.Should().Be(customerId);
            order.Status.Should().Be(nameof(EOrderStatus.Pending));
            order.CurrentVersion.Should().Be(0); // For Event Sourcing, it starts at 0 before commit.

            // Assert that the correct domain event was raised
            order.DomainEvents.Should().HaveCount(1);
            order.DomainEvents.First().Should().BeOfType<OrderCreatedEvent>();
            var createdEvent = order.DomainEvents.First() as OrderCreatedEvent;
            createdEvent?.CustomerId.Should().Be(customerId);
        }

        // --- Testes de Invariantes (Regras de Negócio) ---

        [Fact]
        public void AddItem_WhenOrderIsFilled_ShouldThrowDomainException()
        {
            // Arrange
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));

            // We manually set the state to simulate a filled order to test the invariant.
            order.ChangeState(new FilledOrderState());

            // Act
            Action act = () => order.AddItem("MSFT", 10, Money.Create(100, "USD"));

            // Assert
            // This test proves that the entity protects its own business rules.
            act.Should().Throw<DomainException>()
                .Where(ex => ex.ErrorCode == "orders.cannotModifyWhenClosed");
        }

        [Fact]
        public void AddItem_WithExistingTicker_ShouldThrowDomainException()
        {
            // Arrange
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));
            order.AddItem("MSFT", 10, Money.Create(100, "USD"));

            // Act
            Action act = () => order.AddItem("MSFT", 20, Money.Create(101, "USD")); // Attempt to add the same ticker again

            // Assert
            act.Should().Throw<DomainException>()
                .Where(ex => ex.ErrorCode == "orders.itemAlreadyExists");
        }

        // --- Testes de Interação com Serviços de Domínio ---

        [Fact]
        public async Task Open_WhenValidationSucceeds_ShouldTransitionStateToOpen()
        {
            // Arrange
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));

            // We mock the external dependencies that the method requires.
            var mockValidationService = new Mock<IOrderValidationService>();
            var notificationHandler = new NotificationHandler(); // Using the real one is fine here.

            // Setup the mock to simulate a successful validation (it does nothing and adds no errors).
            mockValidationService
                .Setup(v => v.ValidateForOpening(order, notificationHandler))
                .Returns(Task.CompletedTask);

            // Act
            // We pass the mocked service to the entity's business method.
            await order.Open(mockValidationService.Object, notificationHandler);

            // Assert
            order.Status.Should().Be(nameof(EOrderStatus.Open));
            notificationHandler.HasErrors.Should().BeFalse();

            // Verify that the validation service was actually called.
            mockValidationService.Verify(v => v.ValidateForOpening(order, notificationHandler), Times.Once);
        }

        [Fact]
        public async Task Open_WhenValidationFails_ShouldNotTransitionState_And_HaveNotifications()
        {
            // Arrange
            var order = Order.Create("customer-123", EOrderType.Buy, DateTime.UtcNow.AddDays(1));

            var mockValidationService = new Mock<IOrderValidationService>();
            var notificationHandler = new NotificationHandler();

            // Setup the mock to simulate a FAILED validation by adding an error to the notifier.
            mockValidationService
                .Setup(v => v.ValidateForOpening(It.IsAny<Order>(), It.IsAny<INotificationHandler>()))
                .Callback<Order, INotificationHandler>((o, n) => n.AddError("test.error", "Credit check failed"));

            // Act
            await order.Open(mockValidationService.Object, notificationHandler);

            // Assert
            // The state should NOT have changed.
            order.Status.Should().Be(nameof(EOrderStatus.Pending));
            notificationHandler.HasErrors.Should().BeTrue();
            notificationHandler.Notifications.Should().Contain(n => n.Key == "test.error");
        }
    }
}