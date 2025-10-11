using Foundry.Application.Abstractions.Mappings;
using Foundry.Application.Abstractions.Services;
using Foundry.Domain.Interfaces;
using Foundry.Domain.Notifications;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Requests;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Responses;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges.Validators;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;

namespace Sample.FinancialMarket.Application.Features.Exchanges.Services
{
    /// <summary>
    /// Concrete service for managing the Exchange aggregate.
    /// It inherits from the framework's GenericCrudService to automate CRUD operations
    /// </summary>
    public class ExchangeService : GenericCrudService<Exchange, ExchangeDto, CreateExchangeRequest, UpdateExchangeRequest>, IExchangeService
    {
        public ExchangeService(
            IUnitOfWork unitOfWork,
            IExchangeRepository repository,
            INotificationHandler notifier,
            IMapper<Exchange, ExchangeDto> mapper)
            : base(unitOfWork, repository, notifier, mapper) { }

        protected override Exchange HandleCreate(CreateExchangeRequest dto)
        {
            var exchange = Exchange.Create(dto.Name, dto.Acronym, dto.Country);

            var validator = new ExchangeValidator();
            var validationResult = validator.Validate(exchange);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    Notifier.AddError(error.ErrorCode, error.ErrorMessage);
                }
            }

            return exchange;
        }

        protected override void HandleUpdate(Exchange entity, UpdateExchangeRequest dto)
        {
            entity.UpdateDetails(dto.Name, dto.Country);

        }

    }
}