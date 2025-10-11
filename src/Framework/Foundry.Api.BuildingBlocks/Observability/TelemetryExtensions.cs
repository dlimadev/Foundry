// The code should be in English
using Foundry.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace Foundry.Api.BuildingBlocks.Observability
{
    public static class TelemetryExtensions
    {
        public static IServiceCollection AddFoundryTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = configuration.GetValue<string>("Application:ServiceName") ?? "UnknownService";

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracingProviderBuilder =>
                {
                    tracingProviderBuilder
                        .AddSource("Foundry.Application.Services") // Our custom ActivitySource from the Interceptor
                        .AddAspNetCoreInstrumentation() // Automatically traces incoming HTTP requests
                        .AddHttpClientInstrumentation()   // Automatically traces outgoing HTTP requests
                        //.AddEntityFrameworkCoreInstrumentation(opt => opt.SetDbStatementForText = true) // Traces EF Core DB calls
                        //.AddStackExchangeRedisInstrumentation(); // Traces Redis calls
                    ;

                    // Configures the exporter to send traces to the OTLP endpoint (our APM Server/Collector)
                    tracingProviderBuilder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(configuration["Otel:Exporter:Otlp:Endpoint"]);
                    });
                });

            return services;
        }
    }
}