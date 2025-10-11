namespace Foundry.Domain.Model.Events
{
    /// <summary>
    /// A generic domain event that is raised whenever an entity is updated.
    /// </summary>
    public class EntityUpdatedEvent : IDomainEvent
    {
        /// <summary>
        /// The entity instance that was updated.
        /// </summary>
        public EntityBase Entity { get; }

        public EntityUpdatedEvent(EntityBase entity)
        {
            Entity = entity;
        }
    }
}