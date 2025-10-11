namespace Foundry.Infrastructure.Settings
{
    /// <summary>
    /// Provides strongly-typed configuration for Marten (Event Sourcing with PostgreSQL).
    /// Populated from the "Foundry:EventSourcing:Marten" section of appsettings.json.
    /// </summary>
    public class MartenSettings
    {
        public const string SectionName = "Foundry:EventSourcing:Marten";

        /// <summary>
        /// The connection string for the PostgreSQL database used by Marten.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// The database schema name to use for Marten's event sourcing tables.
        /// </summary>
        public string DatabaseSchemaName { get; set; } = "event_sourcing";
    }
}