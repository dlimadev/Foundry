using Foundry.Domain.Interfaces;
using Foundry.Domain.Services;
using Foundry.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using Sample.FinancialMarket.Infrastructure.Persistence;
using Sample.FinancialMarket.Infrastructure.Persistence.Repositories;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;
using Sample.FinancialMarket.Infrastructure.Providers;
using Sample.FinancialMarket.Domain.Aggregates.Orders;

namespace Sample.FinancialMarket.Infrastructure
{
    /// <summary>
    /// Extension methods for setting up infrastructure services in the DI container for the sample application.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddSampleInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // --- CRUD Persistence Configuration (SQL Server) ---
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<FinanceDbContext>());

            // --- Caching System (Opt-in) ---
            services.AddFoundryCache(configuration);

            // --- Application-Specific Service Implementations ---
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // --- Repository Registration with Interception ---
            // These calls will now be found
            services.AddInterceptedScoped<IStockRepository, StockRepository, Stock>();
            services.AddInterceptedScoped<IPortfolioRepository, PortfolioRepository, Portfolio>();
            services.AddInterceptedScoped<IOrderRepository, OrderEventSourcedRepository, Order>();

            return services;
        }
    }
}