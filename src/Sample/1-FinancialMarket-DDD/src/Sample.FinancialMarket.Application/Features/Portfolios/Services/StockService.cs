using Foundry.Application.Abstractions.Mappings;
using Foundry.Application.Abstractions.Responses;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios.Specifications;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios.Validators;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using System.Net;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Services
{
    /// <summary>
    /// Implements the use cases for the Stock aggregate.
    /// It uses the IMapper pattern for decoupling the mapping logic.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockRepository _stockRepository;
        private readonly INotificationHandler _notifier;
        private readonly IMapper<Stock, StockDto> _stockToDtoV1Mapper;
        private readonly IMapper<Stock, StockDtoV2> _stockToDtoV2Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockService"/> class.
        /// </summary>
        public StockService(
            IUnitOfWork unitOfWork,
            IStockRepository stockRepository,
            INotificationHandler notifier,
            IMapper<Stock, StockDto> stockToDtoV1Mapper,
            IMapper<Stock, StockDtoV2> stockToDtoV2Mapper)
        {
            _unitOfWork = unitOfWork;
            _stockRepository = stockRepository;
            _notifier = notifier;
            _stockToDtoV1Mapper = stockToDtoV1Mapper;
            _stockToDtoV2Mapper = stockToDtoV2Mapper;
        }

        /// <inheritdoc />
        public async Task<Result<StockDto>> CreateStockAsync(CreateStockRequest request)
        {
            var stock = Stock.Create(request.Ticker, request.CompanyName, request.Sector, request.Price, request.MarketCap);

            var validator = new StockValidator();
            var validationResult = await validator.ValidateAsync(stock);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors) _notifier.AddError(error.ErrorCode, error.ErrorMessage);
                return Result<StockDto>.Failure(_notifier.Notifications);
            }

            var spec = new StockByTickerSpecification(request.Ticker);
            if (await _stockRepository.CountAsync(spec) > 0)
            {
                _notifier.AddError("stock.ticker.duplicate", $"A stock with ticker '{request.Ticker}' already exists.");
                return Result<StockDto>.Failure(_notifier.Notifications, HttpStatusCode.Conflict);
            }

            await _stockRepository.AddAsync(stock);
            await _unitOfWork.CompleteAsync();

            var stockDto = _stockToDtoV1Mapper.Map(stock);
            return Result<StockDto>.Success(stockDto, HttpStatusCode.Created);
        }

        /// <inheritdoc />
        public async Task<Result<IReadOnlyList<StockDto>>> GetLargeCapTechStocksAsync()
        {
            var spec = new LargeCapTechStocksSpecification();
            var stocks = await _stockRepository.ListAsync(spec);

            var dtos = stocks.Select(_stockToDtoV1Mapper.Map).ToList().AsReadOnly();
            return Result<IReadOnlyList<StockDto>>.Success(dtos);
        }

        /// <inheritdoc />
        public async Task<Result<StockDto>> GetStockByIdV1Async(Guid id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock == null)
            {
                _notifier.AddError("stock.notFound", $"Stock with ID '{id}' was not found.");
                return Result<StockDto>.Failure(_notifier.Notifications, HttpStatusCode.NotFound);
            }

            return Result<StockDto>.Success(_stockToDtoV1Mapper.Map(stock));
        }

        /// <inheritdoc />
        public async Task<Result<StockDtoV2>> GetStockByIdV2Async(Guid id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock == null)
            {
                _notifier.AddError("stock.notFound", $"Stock with ID '{id}' was not found.");
                return Result<StockDtoV2>.Failure(_notifier.Notifications, HttpStatusCode.NotFound);
            }

            return Result<StockDtoV2>.Success(_stockToDtoV2Mapper.Map(stock));
        }
    }
}