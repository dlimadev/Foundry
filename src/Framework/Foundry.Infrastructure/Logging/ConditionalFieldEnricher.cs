using Serilog.Core;
using Serilog.Events;

namespace Foundry.Infrastructure.Logging
{
    /// <summary>
    /// An enricher that removes specified properties from a log event if the log level
    /// is higher (less detailed) than a specified level (e.g., Debug).
    /// </summary>
    public class ConditionalFieldEnricher : ILogEventEnricher
    {
        private readonly HashSet<string> _debugOnlyProperties;
        private readonly LogEventLevel _minLevelToKeep;

        public ConditionalFieldEnricher(IEnumerable<string> debugOnlyProperties, LogEventLevel minLevelToKeep = LogEventLevel.Debug)
        {
            _debugOnlyProperties = new HashSet<string>(debugOnlyProperties, System.StringComparer.OrdinalIgnoreCase);
            _minLevelToKeep = minLevelToKeep;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // If the log level is Information, Warning, Error, or Fatal (higher than Debug),
            // then remove the debug-only properties.
            if (logEvent.Level > _minLevelToKeep)
            {
                foreach (var propertyName in _debugOnlyProperties)
                {
                    logEvent.RemovePropertyIfPresent(propertyName);
                }
            }
        }
    }
}