using Castle.DynamicProxy;
using Foundry.Domain.Interfaces.Repositories;
using Foundry.Domain.Model;
using Foundry.Infrastructure.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Foundry.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for registering services with interception capabilities using Castle.DynamicProxy.
    /// </summary>
    public static class InterceptorExtensions
    {
        /// <summary>
        /// Registers the core ProxyGenerator from Castle.Core as a singleton service.
        /// This is required for the interception mechanism to work.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the service to.</param>
        /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddFoundryProxyGenerator(this IServiceCollection services)
        {
            services.AddSingleton(new ProxyGenerator());
            return services;
        }

        /// <summary>
        /// Registers a service with a scoped lifetime and applies all registered interceptors (e.g., logging, caching).
        /// This method is the key to the framework's AOP (Aspect-Oriented Programming) capabilities.
        /// </summary>
        /// <typeparam name="TInterface">The service interface (e.g., IStockRepository).</typeparam>
        /// <typeparam name="TImplementation">The concrete service implementation (e.g., StockRepository).</typeparam>
        /// <typeparam name="TEntity">The domain entity type the repository manages (e.g., Stock).</typeparam>
        public static IServiceCollection AddInterceptedScoped<TInterface, TImplementation, TEntity>(this IServiceCollection services)
            where TInterface : class, IGenericRepository<TEntity>
            where TImplementation : class, TInterface
            where TEntity : EntityBase
        {
            // 1. Register the concrete implementation of the service.
            // This is needed so the DI container knows how to build the real object.
            services.AddScoped<TImplementation>();

            // 2. Register the interface using a factory that creates the proxy.
            // When another service asks for TInterface, this factory will be called.
            services.AddScoped(typeof(TInterface), provider =>
            {
                var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
                var implementation = provider.GetRequiredService<TImplementation>();

                // Build a list of all active interceptors.
                var interceptors = new List<IAsyncInterceptor>();

                // Safely get the CachingInterceptor. It will only be registered if caching is enabled.
                var cachingInterceptor = provider.GetService<CachingInterceptor>();
                if (cachingInterceptor != null)
                {
                    // Add Caching first. This ensures that if a cache hit occurs, the request
                    // returns immediately without even hitting the logging interceptor.
                    interceptors.Add(cachingInterceptor);
                }

                // Always add the logging interceptor if it's registered.
                var loggingInterceptor = provider.GetService<LoggingInterceptor>();
                if (loggingInterceptor != null) interceptors.Add(loggingInterceptor);

                // Create the proxy, wrapping the real implementation with the configured interceptors.
                // The proxy object implements TInterface perfectly, solving the type conversion errors.
                return proxyGenerator.CreateInterfaceProxyWithTarget(
                    typeof(TInterface),
                    implementation,
                    interceptors.Cast<IInterceptor>().ToArray()
                );
            });

            return services;
        }
    }
}