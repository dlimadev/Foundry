using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Foundry.Domain.Model.Events;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Foundry.Infrastructure.Caching
{
    /// <summary>
    /// A generic MediatR event handler that listens for entity change events (Created, Updated, Deleted)
    /// and invalidates the relevant cache entries in a distributed cache like Redis.
    /// This provides a decoupled, automatic cache invalidation strategy.
    /// </summary>
    public class EntityChangeCacheInvalidationHandler :
        INotificationHandler<EntityCreatedEvent>,
        INotificationHandler<EntityUpdatedEvent>,
        INotificationHandler<EntityDeletedEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<EntityChangeCacheInvalidationHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityChangeCacheInvalidationHandler"/> class.
        /// </summary>
        public EntityChangeCacheInvalidationHandler(IDistributedCache cache, ILogger<EntityChangeCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task Handle(EntityCreatedEvent notification, CancellationToken cancellationToken)
        {
            // For a created entity, we only need to invalidate list caches, as there is no single-item cache yet.
            return InvalidateCacheForEntity(notification.Entity, "Created");
        }

        /// <inheritdoc />
        public Task Handle(EntityUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return InvalidateCacheForEntity(notification.Entity, "Updated");
        }

        /// <inheritdoc />
        public Task Handle(EntityDeletedEvent notification, CancellationToken cancellationToken)
        {
            return InvalidateCacheForEntity(notification.Entity, "Deleted");
        }

        /// <summary>
        /// Performs the cache invalidation for a given entity.
        /// It first checks if the entity is marked as [Cacheable].
        /// If so, it invalidates both the specific item cache and the list version token for that entity type.
        /// </summary>
        private async Task InvalidateCacheForEntity(EntityBase entity, string action)
        {
            // First, check if the entity is cacheable at all. If not, do nothing.
            bool isCacheable = entity.GetType().IsDefined(typeof(CacheableAttribute), true);
            if (!isCacheable)
            {
                return;
            }

            var entityType = entity.GetType().Name;

            // 1. Invalidate the single item cache (direct key removal).
            // This is crucial for Update and Delete operations.
            var itemKey = $"entity-{entityType}-{entity.Id}";
            await _cache.RemoveAsync(itemKey);
            _logger.LogDebug("Cache invalidation: Removed item key '{CacheKey}' due to entity {Action} event.", itemKey, action);

            // 2. Invalidate ALL list caches for this entity type by removing/updating the version token.
            // This forces any subsequent list query to go back to the database.
            var listVersionKey = $"list-version-{entityType}";
            await _cache.RemoveAsync(listVersionKey);
            _logger.LogDebug("Cache invalidation: Removed list version token '{CacheKey}' for entity type '{EntityType}' due to entity {Action} event.", listVersionKey, entityType, action);
        }
    }
}