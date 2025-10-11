using System.Reflection;

namespace Foundry.Domain.Model
{
    /// <summary>
    /// An optional abstract base class for Aggregate Roots that are persisted using Event Sourcing.
    /// It provides the necessary infrastructure for rehydrating an aggregate from a stream of events
    /// and ensures that state changes only occur through private 'Apply' methods.
    /// </summary>
    public abstract class EventSourcedAggregateRoot : EntityBase, IAggregateRoot
    {
        /// <summary>
        /// Gets the current version of the aggregate after all historical events have been applied.
        /// This is used for optimistic concurrency control in the event store.
        /// </summary>
        public int CurrentVersion { get; protected set; }

        /// <summary>
        /// Applies an event to mutate the aggregate's state and adds it to the list of uncommitted domain events.
        /// This is the primary method used by the aggregate's business logic methods.
        /// </summary>
        /// <param name="domainEvent">The domain event to apply and store.</param>
        protected void ApplyAndStoreEvent(IDomainEvent domainEvent)
        {
            // Invoke the private 'Apply' method corresponding to the event's type.
            InvokeApplyMethod(domainEvent);

            // Add the event to the list of changes to be persisted by the repository.
            AddDomainEvent(domainEvent);
        }

        /// <summary>
        /// Rebuilds the aggregate's state from a historical sequence of events.
        /// This method is called by the Event Sourced repository when loading an aggregate.
        /// </summary>
        /// <param name="history">The stream of past events for this aggregate.</param>
        public void LoadFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var evt in history)
            {
                // When loading from history, we only apply the event to mutate state.
                // We do NOT add it to the list of uncommitted domain events.
                InvokeApplyMethod(evt);
                CurrentVersion++;
            }
        }

        /// <summary>
        /// A helper method that uses reflection to find and invoke the correct private 'Apply' method
        /// for a given event type. This preserves the encapsulation of the aggregate's state.
        /// </summary>
        /// <param name="domainEvent">The event to apply.</param>
        private void InvokeApplyMethod(IDomainEvent domainEvent)
        {
            // Search for a non-public instance method named "Apply" that takes the specific event type as a parameter.
            var applyMethod = GetType().GetMethod(
                "Apply",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { domainEvent.GetType() },
                null);

            // If no such method is found, it's a developer error. The aggregate is incomplete.
            if (applyMethod == null)
            {
                throw new InvalidOperationException(
                    $"The aggregate '{GetType().Name}' is missing a private 'Apply({domainEvent.GetType().Name})' method.");
            }

            // Invoke the found 'Apply' method on the current instance.
            applyMethod.Invoke(this, new[] { (object)domainEvent });
        }

        /// <summary>
        /// When implemented in a derived class, creates a snapshot of the aggregate's current state.
        /// This must be public to be called by the repository.
        /// </summary>
        public abstract IEventSourcedSnapshot CreateSnapshot();

        /// <summary>
        /// When implemented in a derived class, restores the aggregate's state from a snapshot.
        /// This must be public to be called by the repository.
        /// </summary>
        public abstract void RestoreFromSnapshot(IEventSourcedSnapshot snapshot);
    }
}