using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Mappings
{
    public class FinancialAssetToDtoMapper : IMapper<FinancialAsset, FinancialAssetDto>
    {
        public FinancialAssetDto Map(FinancialAsset source)
        {
            if (source == null) return null!;
            var children = (source is Portfolio composite) ? composite.Assets.Select(Map).ToList() : new List<FinancialAssetDto>();
            return new FinancialAssetDto(source.Id, source.Ticker, source.GetType().Name, source.Price, children);
        }
    }
}