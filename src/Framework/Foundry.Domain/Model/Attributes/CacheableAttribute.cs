namespace Foundry.Domain.Model.Attributes
{
    /// <summary>
    /// When applied to an entity class, indicates that its repository's read operations should be cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CacheableAttribute : Attribute
    {
        /// <summary>
        /// Gets the cache duration in minutes.
        /// </summary>
        public int DurationInMinutes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheableAttribute"/> class.
        /// </summary>
        /// <param name="durationInMinutes">The cache duration in minutes. Defaults to 5 minutes.</param>
        public CacheableAttribute(int durationInMinutes = 5)
        {
            if (durationInMinutes <= 0)
                throw new ArgumentException("Cache duration must be positive.", nameof(durationInMinutes));
            DurationInMinutes = durationInMinutes;
        }
    }
}