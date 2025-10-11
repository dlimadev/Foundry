using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application.Features.Portfolios.Mappings
{
    public class StockToStockDtoMapper : IMapper<Stock, StockDto>
    {
        public StockDto Map(Stock source)
        {
            if (source == null) return null!;
            return new StockDto(
                        source.Id,
                        source.Ticker,
                        source.CompanyName,
                        source.Sector, 
                        source.Price,
                        source.MarketCap
             );
        }
    }
}