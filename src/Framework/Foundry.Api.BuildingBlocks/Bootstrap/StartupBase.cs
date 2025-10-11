using Foundry.Api.BuildingBlocks.Extensions;
using Foundry.Api.BuildingBlocks.Middlewares;
using Foundry.Domain.Interfaces.Auditing;
using Foundry.Infrastructure.Auditing;
using Foundry.Infrastructure.Auditing.Implementations;
using Foundry.Infrastructure.DependencyInjection;
using Foundry.Infrastructure.Logging;
using Foundry.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Foundry.Api.BuildingBlocks.Bootstrap
{
    /// <summary>
    /// An abstract base startup class that provides a standardized, opinionated
    /// way to configure services and the HTTP pipeline for applications using the Foundry framework.
    /// It uses a "convention over configuration" approach with virtual methods for extensibility.
    /// </summary>
    public abstract class StartupBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly IWebHostEnvironment Environment;

        protected StartupBase(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// This is the main, sealed method that orchestrates the entire service configuration.
        /// It enforces the registration order of framework and application services.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // --- Framework Mandatory Services ---
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddFoundryProxyGenerator();
            services.AddFoundryObservability(Configuration);
            services.AddFoundryApiVersioning();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // --- Configurable Framework Services (with default implementations) ---
            ConfigureAuditing(services);
            ConfigureCaching(services);

            // --- Application-Specific Services (must be implemented by the child class) ---
            ConfigureApplicationServices(services);
        }

        /// <summary>
        /// This is the main, sealed method that orchestrates the HTTP request pipeline.
        /// It enforces the correct middleware order for security, observability, and functionality.
        /// </summary>
        public void Configure(IApplicationBuilder app)
        {
            app.UseFoundryExceptionHandler(Environment);

            if (Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            // app.UseAuthentication(); // Placeholder for where authentication would go
            app.UseAuthorization();

            // Foundry framework middlewares
            app.UseRequestLocalization();
            app.UseFoundryCorrelationId();
            app.UseFoundryUserContextLog();
            app.UseFoundryResponseHeaders();

            // Hook for the consuming application to add its own custom middlewares
            ConfigureCustomMiddlewares(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // --- ABSTRACT & VIRTUAL METHODS FOR THE CONSUMING APP TO IMPLEMENT/OVERRIDE ---

        /// <summary>
        /// (ABSTRACT) The consuming application MUST implement this method to register its own services
        /// (e.g., from its own Application and Infrastructure layers).
        /// </summary>
        protected abstract void ConfigureApplicationServices(IServiceCollection services);

        /// <summary>
        /// (VIRTUAL) Configures the Auditing strategy based on appsettings.json.
        /// The framework's default is to use a dedicated database.
        /// The consuming application can override this method to choose a different strategy (Kafka, File, etc.).
        /// </summary>
        protected virtual void ConfigureAuditing(IServiceCollection services)
        {
            var auditSettings = new AuditingSettings();
            Configuration.GetSection(AuditingSettings.SectionName).Bind(auditSettings);

            switch (auditSettings.StoreType)
            {
                case AuditStoreType.Database:
                    services.AddDbContext<AuditDbContext>(options =>
                        options.UseSqlServer(auditSettings.Database?.ConnectionString));
                    services.AddScoped<IAuditLogStore, EfCoreAuditLogStore>();
                    break;
                case AuditStoreType.File:
                    services.AddSingleton<IAuditLogStore, FileAuditLogStore>();
                    break;
                case AuditStoreType.Kafka:
                    // Requires IProducer to be registered, usually in ConfigureApplicationServices
                    services.AddScoped<IAuditLogStore, KafkaAuditLogStore>();
                    break;
                case AuditStoreType.None:
                default:
                    services.AddScoped<IAuditLogStore, NullAuditLogStore>();
                    break;
            }
        }

        /// <summary>
        /// (VIRTUAL) Configures the Distributed Cache strategy based on appsettings.json.
        /// The consuming application can override this to change options or use a different provider.
        /// </summary>
        protected virtual void ConfigureCaching(IServiceCollection services)
        {
            // This call to the extension method reads the "Foundry:Cache:Redis" section
            // and enables caching only if IsEnabled = true.
            services.AddFoundryCache(Configuration);
        }

        /// <summary>
        /// (VIRTUAL) A hook for the consuming application to add its own custom middlewares to the HTTP pipeline.
        /// </summary>
        protected virtual void ConfigureCustomMiddlewares(IApplicationBuilder app) { }
    }
}