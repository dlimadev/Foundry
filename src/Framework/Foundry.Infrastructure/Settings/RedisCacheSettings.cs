namespace Foundry.Infrastructure.Settings
{
    /// <summary>
    /// Provides strongly-typed configuration for the Redis distributed cache.
    /// Populated from the "Foundry:Cache:Redis" section of appsettings.json.
    /// </summary>
    public class RedisCacheSettings
    {
        public const string SectionName = "Foundry:Cache:Redis";

        /// <summary>
        /// A flag to enable or disable the Redis cache feature for the application.
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// The actual connection string for the Redis instance (e.g., "localhost:6379").
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// The instance name prefix to be used for all cache keys to prevent collisions.
        /// </summary>
        public string InstanceName { get; set; } = "FoundryApp_";

        /// <summary>
        /// The password for the Redis instance. Should be loaded from a secure source.
        /// </summary>
        public string? Password { get; set; }
    }
}