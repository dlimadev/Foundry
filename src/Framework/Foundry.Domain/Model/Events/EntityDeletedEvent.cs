namespace Foundry.Domain.Model.Events
{
    /// <summary>
    /// A generic domain event that is raised whenever an entity is deleted.
    /// </summary>
    public class EntityDeletedEvent : IDomainEvent
    {
        /// <summary>
        /// The entity instance that was deleted.
        /// </summary>
        public EntityBase Entity { get; }

        public EntityDeletedEvent(EntityBase entity)
        {
            Entity = entity;
        }
    }
}