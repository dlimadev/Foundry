using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.Model
{
    /// <summary>
    /// Abstract base class for all domain entities.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary> The unique identifier for the entity (GUID). </summary>
        public Guid Id { get; protected set; }

        /// <summary> The UTC date and time when the entity was created. </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary> The identifier of the user who created the entity. </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary> The UTC date and time when the entity was last modified. </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary> The identifier of the user who last modified the entity. </summary>
        public string? LastModifiedBy { get; set; }

        /// <summary> A version number for optimistic concurrency control. </summary>
        [Timestamp]
        public byte[]? Version { get; set; }

        /// <summary> Flag to indicate if the entity has been "soft-deleted". </summary>
        public bool IsDeleted { get; set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary> A read-only collection of domain events raised by this entity. </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Adds a domain event to the entity's list of pending events.
        /// This is public to allow infrastructure layers (like a DbContext) to raise cross-cutting events.
        /// </summary>
        /// <param name="domainEvent">The domain event to add.</param>
        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        /// <summary>
        /// Clears the list of pending domain events.
        /// This is called by the Unit of Work after the events have been dispatched.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();

        // --- Equality Operators ---
        public override bool Equals(object? obj)
        {
            if (obj is not EntityBase other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id.Equals(Guid.Empty) || other.Id.Equals(Guid.Empty)) return false;
            return Id.Equals(other.Id);
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(EntityBase a, EntityBase b) => !(a == b);
        public override int GetHashCode() => Id.GetHashCode();
    }
}