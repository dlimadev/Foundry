using Foundry.Application.Abstractions.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Sample.FinancialMarket.Application.Features.Exchanges.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Exchanges.Mappings;
using Sample.FinancialMarket.Application.Features.Orders.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Orders.Mappings;
using Sample.FinancialMarket.Application.Features.Orders.Services;
using Sample.FinancialMarket.Application.Features.Orders.Validation;
using Sample.FinancialMarket.Application.Features.Portfolios.Dtos.Responses;
using Sample.FinancialMarket.Application.Features.Portfolios.Mappings;
using Sample.FinancialMarket.Application.Features.Portfolios.Services;
using Sample.FinancialMarket.Domain.Aggregates.Exchanges;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Aggregates.Orders.Services;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;

namespace Sample.FinancialMarket.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSampleApplication(this IServiceCollection services)
        {
            // --- Register Mappers ---
            services.AddScoped<IMapper<Exchange, ExchangeDto>, ExchangeToDtoMapper>();
            services.AddScoped<IMapper<Order, OrderDto>, OrderToOrderDtoMapper>();
            services.AddScoped<IMapper<Stock, StockDto>, StockToStockDtoMapper>();
            services.AddScoped<IMapper<Stock, StockDtoV2>, StockToStockDtoV2Mapper>();
            services.AddScoped<IMapper<Portfolio, PortfolioDto>, PortfolioToDtoMapper>();
            services.AddScoped<IMapper<FinancialAsset, FinancialAssetDto>, FinancialAssetToDtoMapper>();

            // Register domain service implementations
            services.AddScoped<IOrderValidationService, OrderValidationService>();

            // Register application services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IPortfolioService, PortfolioService>();

            return services;
        }
    }
}