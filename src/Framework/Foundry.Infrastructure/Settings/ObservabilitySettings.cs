// The code should be in English
namespace Foundry.Infrastructure.Settings
{
    /// <summary>
    /// Provides strongly-typed configuration for observability (OpenTelemetry).
    /// Populated from the "Foundry:Observability" section of appsettings.json.
    /// </summary>
    public class ObservabilitySettings
    {
        public const string SectionName = "Foundry:Observability";

        /// <summary>
        /// The name of the service, to be used in traces and logs.
        /// </summary>
        public string ServiceName { get; set; } = "UnnamedService";

        /// <summary>
        /// The endpoint URL for the OTLP (OpenTelemetry Protocol) exporter (e.g., APM Server or OTel Collector).
        /// </summary>
        public string? OtlpExporterEndpoint { get; set; }
    }
}