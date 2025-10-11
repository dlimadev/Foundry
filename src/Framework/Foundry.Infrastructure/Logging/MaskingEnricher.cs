using Serilog.Core;
using Serilog.Events;

namespace Foundry.Infrastructure.Logging
{
    public class MaskingEnricher : ILogEventEnricher
    {
        private readonly HashSet<string> _propertiesToMask;
        private const string MaskValue = "*** MASKED ***";

        // A hardcoded list of universally sensitive property names.
        // The framework enforces this list by default.
        private static readonly HashSet<string> DefaultPropertiesToMask = new(System.StringComparer.OrdinalIgnoreCase)
        {
            "Password", "CreditCardNumber", "CardNumber", "CVV", "Cvc", "Token", "Authorization",
            "NIF", "SocialSecurityNumber", "SSN" 
        };

        public MaskingEnricher(IEnumerable<string>? userDefinedPropertiesToMask = null)
        {
            // Start with the framework's default list.
            _propertiesToMask = new HashSet<string>(DefaultPropertiesToMask, System.StringComparer.OrdinalIgnoreCase);

            // Add any custom properties defined by the consuming application.
            if (userDefinedPropertiesToMask != null)
            {
                _propertiesToMask.UnionWith(userDefinedPropertiesToMask);
            }
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // A lógica de mascaramento recursivo permanece a mesma...
            foreach (var property in logEvent.Properties.ToList())
            {
                var maskedProperty = MaskProperty(property.Key, property.Value, propertyFactory);
                if (maskedProperty != null)
                {
                    logEvent.AddOrUpdateProperty(maskedProperty);
                }
            }
        }

        private LogEventProperty? MaskProperty(string propertyName, LogEventPropertyValue propertyValue, ILogEventPropertyFactory propertyFactory)
        {
            if (_propertiesToMask.Contains(propertyName))
            {
                return propertyFactory.CreateProperty(propertyName, MaskValue);
            }

            if (propertyValue is StructureValue structure)
            {
                var maskedSubProperties = structure.Properties.Select(prop => MaskProperty(prop.Name, prop.Value, propertyFactory) ?? prop);
                return propertyFactory.CreateProperty(propertyName, new StructureValue(maskedSubProperties));
            }

            return null;
        }
    }
}