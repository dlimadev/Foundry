using Foundry.Infrastructure.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Foundry.Infrastructure.Logging
{
    public static class ObservabilityExtensions
    {
        /// <summary>
        /// Registers the core observability services from the framework, including
        /// a singleton ActivitySource based on the application's configuration.
        /// </summary>
        public static IServiceCollection AddFoundryObservability(this IServiceCollection services, IConfiguration configuration)
        {
            // Read the service name from the consuming application's configuration.
            var serviceName = configuration.GetValue<string>("Application:ServiceName") ?? "UnknownService";

            // Create a single ActivitySource for the entire application and register it as a singleton.
            var activitySource = new ActivitySource(serviceName);
            services.AddSingleton(activitySource);

            // Also register our interceptor, which will use the ActivitySource.
            services.AddScoped<LoggingInterceptor>();

            return services;
        }
    }
}