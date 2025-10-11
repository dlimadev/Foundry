using Foundry.Domain.Model;

namespace Foundry.Domain.Interfaces.EventSourcing
{
    /// <summary>
    /// Defines the contract for an Event Store, which is responsible for persisting and retrieving
    /// streams of domain events for an aggregate. This interface belongs to the Domain
    /// as it defines a contract required by the domain's persistence ignorance.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Saves a new batch of events to an aggregate's stream.
        /// </summary>
        /// <param name="aggregateId">The ID of the aggregate.</param>
        /// <param name="events">The collection of new domain events to save.</param>
        /// <param name="expectedVersion">The version of the aggregate before these new events, used for optimistic concurrency.</param>
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);

        /// <summary>
        /// Retrieves the event history for an aggregate starting from a specific version.
        /// </summary>
        /// <param name="aggregateId">The ID of the aggregate.</param>
        /// <param name="fromVersion">The version from which to start loading events. Use 0 to load all events.</param>
        /// <returns>A list of domain events.</returns>
        Task<List<IDomainEvent>> GetEventsForAggregateAsync(Guid aggregateId, int fromVersion);

        /// <summary>
        /// Saves a snapshot of an aggregate's state.
        /// </summary>
        /// <param name="snapshot">The snapshot object to save.</param>
        Task SaveSnapshotAsync(IEventSourcedSnapshot snapshot);

        /// <summary>
        /// Retrieves the most recent snapshot for an aggregate.
        /// </summary>
        /// <param name="aggregateId">The ID of the aggregate.</param>
        /// <returns>The latest snapshot, or null if none exists.</returns>
        Task<IEventSourcedSnapshot?> GetLatestSnapshotAsync(Guid aggregateId);
    }
}