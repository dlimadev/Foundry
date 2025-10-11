using FluentAssertions;
using Foundry.Application.Abstractions.Mappings;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Moq;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Portfolios.Services;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios.Specifications;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using System.Net;

namespace Sample.FinancialMarket.Application.Tests.Features.Portfolios.Services
{
    /// <summary>
    /// Unit tests for the StockService.
    /// These tests verify the orchestration logic of the service, mocking its dependencies.
    /// </summary>
    public class StockServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStockRepository> _mockStockRepository;
        private readonly INotificationHandler _notificationHandler;
        private readonly Mock<IMapper<Stock, StockDto>> _mockStockMapperV1;
        private readonly Mock<IMapper<Stock, StockDtoV2>> _mockStockMapperV2;
        private readonly StockService _stockService;

        public StockServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStockRepository = new Mock<IStockRepository>();
            _notificationHandler = new NotificationHandler();
            _mockStockMapperV1 = new Mock<IMapper<Stock, StockDto>>();
            _mockStockMapperV2 = new Mock<IMapper<Stock, StockDtoV2>>();

            // Instantiate the service under test, injecting the mocked dependencies.
            _stockService = new StockService(
                _mockUnitOfWork.Object,
                _mockStockRepository.Object,
                _notificationHandler,
                _mockStockMapperV1.Object,
                _mockStockMapperV2.Object);
        }

        [Fact]
        public async Task CreateStockAsync_WithValidData_ShouldSucceedAndCallRepositoryAndUnitOfWork()
        {
            // Arrange
            var request = new CreateStockRequest("GOOGL", "Alphabet Inc.", "Technology", 2800, 2_000_000_000_000m);
            var createdStock = Stock.Create(request.Ticker, request.CompanyName, request.Sector, request.Price, request.MarketCap);
            var expectedDto = new StockDto(createdStock.Id, createdStock.Ticker, createdStock.CompanyName, createdStock.Sector, createdStock.Price, createdStock.MarketCap);

            // We simulate that the stock does not exist yet.
            _mockStockRepository.Setup(r => r.CountAsync(It.IsAny<StockByTickerSpecification>())).ReturnsAsync(0);

            // We simulate the mapping from entity to DTO.
            _mockStockMapperV1.Setup(m => m.Map(It.IsAny<Stock>())).Returns(expectedDto);

            // Act
            var result = await _stockService.CreateStockAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedDto);
            result.SuggestedStatusCode.Should().Be(HttpStatusCode.Created);

            // Verify: This is the core of the test. We ensure the service orchestrated correctly.
            // 1. Check that a new stock was added to the repository.
            _mockStockRepository.Verify(repo => repo.AddAsync(It.IsAny<Stock>()), Times.Once);
            // 2. Check that the transaction was committed.
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStockAsync_WithDuplicateTicker_ShouldReturnConflictAndNotPersist()
        {
            // Arrange
            var request = new CreateStockRequest("MSFT", "Microsoft", "Technology", 300, 2_000_000_000_000m);

            // We simulate that a stock with this ticker ALREADY exists.
            _mockStockRepository.Setup(r => r.CountAsync(It.IsAny<StockByTickerSpecification>())).ReturnsAsync(1);

            // Act
            var result = await _stockService.CreateStockAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.SuggestedStatusCode.Should().Be(HttpStatusCode.Conflict);
            _notificationHandler.Notifications.Should().Contain(n => n.Key == "stock.ticker.duplicate");

            // Verify: Crucially, we ensure no data was saved to the database.
            _mockStockRepository.Verify(repo => repo.AddAsync(It.IsAny<Stock>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateStockAsync_WithInvalidData_ShouldReturnFailureAndNotPersist()
        {
            // Arrange
            var request = new CreateStockRequest("", "Invalid Company", "Technology", 0, 0); 

            // Act
            var result = await _stockService.CreateStockAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.SuggestedStatusCode.Should().BeNull(); // No specific status code for simple validation failure
            _notificationHandler.Notifications.Should().Contain(n => n.Key == "stock.ticker.required");
            _notificationHandler.Notifications.Should().Contain(n => n.Key == "stock.price.invalid");

            // Verify: Ensure no database operations were attempted with invalid data.
            _mockStockRepository.Verify(repo => repo.AddAsync(It.IsAny<Stock>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}