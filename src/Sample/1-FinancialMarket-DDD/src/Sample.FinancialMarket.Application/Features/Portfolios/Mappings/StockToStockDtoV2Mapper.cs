using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Mappings
{
    public class StockToStockDtoV2Mapper : IMapper<Stock, StockDtoV2>
    {
        public StockDtoV2 Map(Stock source)
        {
            if (source == null) return null!;
            return new StockDtoV2(source.Id, source.Ticker, source.CompanyName, source.Price, "Strong Buy");
        }
    }
}