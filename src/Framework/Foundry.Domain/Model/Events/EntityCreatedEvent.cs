namespace Foundry.Domain.Model.Events
{
    /// <summary>
    /// A generic domain event that is raised whenever a new entity is created.
    /// Used for cross-cutting concerns like cache invalidation.
    /// </summary>
    public class EntityCreatedEvent : IDomainEvent
    {
        /// <summary>
        /// The entity instance that was created.
        /// </summary>
        public EntityBase Entity { get; }

        public EntityCreatedEvent(EntityBase entity)
        {
            Entity = entity;
        }
    }
}