using Foundry.Application.Abstractions.Responses;
using Foundry.Application.Abstractions.Services;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Responses;

namespace Sample.FinancialMarket.Application.Features.Exchanges.Services
{
    public interface IExchangeService: IGenericCrudService<ExchangeDto, CreateExchangeRequest, UpdateExchangeRequest>
    {
        Task<Result<ExchangeDto>> GetByIdAsync(Guid id);
        Task<Result<ExchangeDto>> CreateAsync(CreateExchangeRequest dto);
        Task<Result<ExchangeDto>> UpdateAsync(Guid id, UpdateExchangeRequest dto);
    }
}