namespace Foundry.Infrastructure.Settings
{
    /// <summary>
    /// Represents the available storage strategies for audit logs.
    /// </summary>
    public enum AuditStoreType
    {
        None,
        Database,
        Kafka,
        File
    }

    /// <summary>
    /// Provides strongly-typed configuration options for the auditing system.
    /// Populated from the "Foundry:Auditing" section of appsettings.json.
    /// </summary>
    public class AuditingSettings
    {
        public const string SectionName = "Foundry:Auditing";

        /// <summary>
        /// The chosen strategy for storing audit logs. This is a required setting.
        /// </summary>
        public AuditStoreType StoreType { get; set; } = AuditStoreType.None;

        /// <summary>
        /// Configuration settings if the 'Kafka' StoreType is chosen.
        /// </summary>
        public KafkaSettings? Kafka { get; set; }

        /// <summary>
        /// Configuration settings if the 'File' StoreType is chosen.
        /// </summary>
        public FileSettings? File { get; set; }

        /// <summary>
        /// Configuration settings if the 'Database' StoreType is chosen.
        /// </summary>
        public DatabaseSettings? Database { get; set; }

        public class KafkaSettings { public string TopicName { get; set; } = "foundry-audit-logs"; }
        public class FileSettings { public string FilePath { get; set; } = "logs/audit.log"; }
        public class DatabaseSettings { public string? ConnectionString { get; set; } }
    }
}