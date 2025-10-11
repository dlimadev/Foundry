// The code should be in English
using System.Collections.Generic;

namespace Foundry.Infrastructure.Settings
{
    /// <summary>
    /// Provides strongly-typed configuration for Serilog's custom masking enrichers.
    /// Populated from the "Foundry:Logging" section of appsettings.json.
    /// </summary>
    public class LoggingMaskingSettings
    {
        public const string SectionName = "Foundry:Logging";

        /// <summary>
        /// A list of application-specific property names to be masked in logs.
        /// </summary>
        public List<string> PropertiesToMask { get; set; } = new();

        /// <summary>
        /// A list of properties that should only be logged in Debug/Verbose levels.
        /// </summary>
        public List<string> DebugOnlyProperties { get; set; } = new();
    }
}