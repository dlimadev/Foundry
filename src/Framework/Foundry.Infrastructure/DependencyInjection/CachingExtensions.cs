using Foundry.Infrastructure.Interceptors;
using Foundry.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Foundry.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for configuring distributed caching and enabling the caching interceptor.
    /// </summary>
    public static class CachingExtensions
    {
        /// <summary>
        /// Registers the distributed cache services (Redis) for the framework if enabled in the configuration.
        /// This method is the entry point for an application to "opt-in" to the caching feature.
        /// It also registers the CachingInterceptor.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        /// <param name="configuration">The application's configuration.</param>
        /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddFoundryCache(this IServiceCollection services, IConfiguration configuration)
        {
            // Binds the "Foundry:Cache:Redis" section from appsettings.json to our settings class.
            var cacheSettings = new RedisCacheSettings();
            configuration.GetSection(RedisCacheSettings.SectionName).Bind(cacheSettings);

            // If caching is not explicitly enabled in the configuration, do nothing.
            if (!cacheSettings.IsEnabled)
            {
                return services;
            }

            if (string.IsNullOrEmpty(cacheSettings.ConnectionString))
            {
                throw new InvalidOperationException($"Redis caching is enabled, but the connection string is missing from the configuration ('{RedisCacheSettings.SectionName}:ConnectionString').");
            }

            // Register the CachingInterceptor, making it available to the DI container.
            // Our AddInterceptedScoped method will look for this service.
            services.AddScoped<CachingInterceptor>();

            // Register the Redis distributed cache services.
            services.AddStackExchangeRedisCache(options =>
            {
                var configOptions = ConfigurationOptions.Parse(cacheSettings.ConnectionString);

                if (!string.IsNullOrEmpty(cacheSettings.Password))
                {
                    configOptions.Password = cacheSettings.Password;
                }

                options.ConfigurationOptions = configOptions;
                options.InstanceName = cacheSettings.InstanceName;
            });

            return services;
        }
    }
}