using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Mappings
{
    public class PortfolioToDtoMapper : IMapper<Portfolio, PortfolioDto>
    {
        public PortfolioDto Map(Portfolio source)
        {
            if (source == null) return null!;
            return new PortfolioDto(source.Id, source.Ticker, source.Name, source.Description, source.Assets.Count);
        }
    }
}