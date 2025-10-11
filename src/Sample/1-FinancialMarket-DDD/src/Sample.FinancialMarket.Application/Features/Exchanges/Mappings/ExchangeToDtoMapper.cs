using Foundry.Application.Abstractions.Mappings;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;

namespace Sample.FinancialMarket.Application.Features.Exchanges.Mappings
{
    public class ExchangeToDtoMapper : IMapper<Exchange, ExchangeDto>
    {
        public ExchangeDto Map(Exchange source)
        {
            if (source == null) return null!;
            return new ExchangeDto(source.Id, source.Name, source.Acronym, source.Country);
        }
    }
}