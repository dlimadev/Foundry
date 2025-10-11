using Foundry.Domain.Interfaces.Auditing;
using Foundry.Infrastructure.Auditing;
using Foundry.Infrastructure.Auditing.Implementations;
using Foundry.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Foundry.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddFoundryInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<AuditingSettings> auditOptions)
        {
            var options = new AuditingSettings();
            auditOptions(options);

            // --- Configure Auditing Strategy ---
            switch (options.StoreType)
            {
                case AuditStoreType.Database:
                    // Registers the DbContext for auditing and the EF Core store implementation.
                    services.AddDbContext<AuditDbContext>(dbOptions =>
                        dbOptions.UseSqlServer(configuration.GetConnectionString("Auditing:ConnectionStrings:AuditDbConnection")));
                    services.AddScoped<IAuditLogStore, EfCoreAuditLogStore>();
                    break;

                case AuditStoreType.Kafka:
                    // Registers the Kafka producer and the Kafka store implementation.
                    // (Requires Kafka settings in appsettings and IProducer registration)
                    services.AddScoped<IAuditLogStore, KafkaAuditLogStore>();
                    break;

                case AuditStoreType.File:
                    // Registers the file-based store implementation.
                    services.AddSingleton<IAuditLogStore, FileAuditLogStore>();
                    break;

                case AuditStoreType.None:
                    // Registers a "do nothing" implementation if auditing is explicitly disabled.
                    services.AddScoped<IAuditLogStore, NullAuditLogStore>();
                    break;

                default:
                    // Throws an exception at startup if no valid audit store is configured.
                    throw new InvalidOperationException("A valid AuditStoreType must be configured.");
            }


            return services;
        }
    }
}