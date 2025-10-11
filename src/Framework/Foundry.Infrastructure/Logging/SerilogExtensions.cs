using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Foundry.Infrastructure.Logging
{
    /// <summary>
    /// Provides extension methods to easily configure and bootstrap Serilog logging
    /// with the Foundry framework's recommended settings.
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Configures Serilog for the application host, applying Foundry's default opinions
        /// for security and clarity, such as data masking and conditional field logging.
        /// </summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <returns>The host builder so that additional calls can be chained.</returns>
        public static IHostBuilder UseFoundrySerilog(this IHostBuilder builder)
        {
            // CORRECTION: Using clear and distinct names for the lambda parameters:
            // 'hostContext' for the application's context.
            // 'loggerConfiguration' for the Serilog configuration object.
            builder.UseSerilog((hostContext, services, loggerConfiguration) =>
            {
                // --- Step 1: Read application-specific settings from appsettings.json ---
                // We use 'hostContext.Configuration' to read the application's general configuration.
                var userDefinedMasking = hostContext.Configuration.GetSection("LoggingSettings:PropertiesToMask").Get<List<string>>();
                var debugOnlyProperties = hostContext.Configuration.GetSection("LoggingSettings:DebugOnlyProperties").Get<List<string>>();

                // --- Step 2: Build the logger configuration ---
                // All chained calls here are made on the 'loggerConfiguration' object.
                loggerConfiguration
                    // Read base configuration (minimum levels, etc.) from appsettings.json
                    // This is the corrected line: it uses 'hostContext.Configuration' as the source.
                    .ReadFrom.Configuration(hostContext.Configuration)
                    
                    // Allows Serilog to use services from the DI container if needed
                    .ReadFrom.Services(services)
                    
                    // Enriches logs with properties from the current context (e.g., CorrelationId)
                    .Enrich.FromLogContext()

                    // --- Step 3: Apply Framework-Enforced Enrichers ---
                    .Enrich.With(new MaskingEnricher(userDefinedMasking))
                    .Enrich.With(new ConditionalFieldEnricher(debugOnlyProperties ?? new List<string>()))
                    
                    // --- Step 4: Apply Framework-Enforced Opinions ---
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)

                    // --- Step 5: Define a Default Sink ---
                    .WriteTo.Console(new JsonFormatter());
            });

            return builder;
        }
    }
}