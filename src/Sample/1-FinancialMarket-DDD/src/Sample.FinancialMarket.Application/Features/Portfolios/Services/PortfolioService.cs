// The code should be in English
using Foundry.Application.Abstractions.Mappings;
using Foundry.Application.Abstractions.Responses;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios.Validators;
using Sample.FinancialMarket.Domain.Interfaces;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly INotificationHandler _notifier;
        private readonly IMapper<Portfolio, PortfolioDto> _portfolioMapper;
        private readonly IMapper<FinancialAsset, FinancialAssetDto> _structureMapper;

        public PortfolioService(
            IUnitOfWork unitOfWork,
            IPortfolioRepository portfolioRepository,
            INotificationHandler notifier,
            IMapper<Portfolio, PortfolioDto> portfolioMapper,
            IMapper<FinancialAsset, FinancialAssetDto> structureMapper)
        {
            _unitOfWork = unitOfWork;
            _portfolioRepository = portfolioRepository;
            _notifier = notifier;
            _portfolioMapper = portfolioMapper;
            _structureMapper = structureMapper;
        }

        public async Task<Result<PortfolioDto>> CreatePortfolioAsync(CreatePortfolioRequest request)
        {
            var portfolio = Portfolio.Create(request.Ticker, request.Name, request.Description);

            var validator = new PortfolioValidator();
            var validationResult = await validator.ValidateAsync(portfolio);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _notifier.AddError(error.ErrorCode, error.ErrorMessage);
                }
                return Result<PortfolioDto>.Failure(_notifier.Notifications);
            }

            await _portfolioRepository.AddAsync(portfolio);
            await _unitOfWork.CompleteAsync();

            return Result<PortfolioDto>.Success(_portfolioMapper.Map(portfolio), HttpStatusCode.Created);
        }

        public async Task<Result<FinancialAssetDto>> GetPortfolioStructureAsync(Guid portfolioId)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
            if (portfolio == null)
            {
                _notifier.AddError("portfolio.notFound", "Portfolio not found.");
                return Result<FinancialAssetDto>.Failure(_notifier.Notifications, HttpStatusCode.NotFound);
            }

            return Result<FinancialAssetDto>.Success(_structureMapper.Map(portfolio));
        }
    }
}