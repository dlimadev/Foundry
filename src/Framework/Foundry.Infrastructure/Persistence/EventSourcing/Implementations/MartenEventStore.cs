using Foundry.Domain.Interfaces.EventSourcing;
using Foundry.Domain.Model;
using Marten;

namespace Foundry.Infrastructure.Persistence.EventSourcing.Implementations
{
    /// <summary>
    /// A production-ready implementation of the IEventStore contract using the Marten library.
    /// It leverages PostgreSQL as its underlying storage engine for events and snapshots.
    /// </summary>
    public class MartenEventStore : IEventStore
    {
        private readonly IDocumentStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="MartenEventStore"/> class.
        /// </summary>
        /// <param name="store">The Marten IDocumentStore, which is the main entry point to the library, provided via Dependency Injection.</param>
        public MartenEventStore(IDocumentStore store)
        {
            _store = store;
        }

        /// <inheritdoc />
        public async Task<List<IDomainEvent>> GetEventsForAggregateAsync(Guid aggregateId, int fromVersion)
        {
            // A session in Marten is a unit of work.
            await using var session = _store.LightweightSession();

            // Marten's FetchStreamAsync can load events starting from a specific version.
            var events = await session.Events.FetchStreamAsync(aggregateId, version: fromVersion);

            // The .Data property of Marten's IEvent contains our actual domain event object.
            return events.Select(e => (IDomainEvent)e.Data).ToList();
        }

        /// <inheritdoc />
        public async Task<IEventSourcedSnapshot?> GetLatestSnapshotAsync(Guid aggregateId)
        {
            await using var session = _store.LightweightSession();

            // Marten can store snapshots as plain documents. We use its powerful LINQ provider to query for the latest one.
            var snapshot = await session.Query<IEventSourcedSnapshot>()
                .Where(s => s.AggregateId == aggregateId)
                .OrderByDescending(s => s.Version)
                .FirstOrDefaultAsync();

            return snapshot;
        }

        /// <inheritdoc />
        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
        {
            // We use a lightweight session for write operations.
            await using var session = _store.LightweightSession();

            // Marten's Append method handles optimistic concurrency checks using the expectedVersion.
            // If the current stream version in the DB is not 'expectedVersion', it will throw a ConcurrencyException.
            // The events are cast to 'object' as Marten's API requires.
            session.Events.Append(aggregateId, expectedVersion, events.Cast<object>().ToArray());

            // The changes (new events) are only sent to the database when SaveChangesAsync is called.
            await session.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveSnapshotAsync(IEventSourcedSnapshot snapshot)
        {
            await using var session = _store.LightweightSession();

            // Marten can store snapshots as regular documents.
            // The 'Store' operation will either insert a new document or update an existing one.
            session.Store(snapshot);

            await session.SaveChangesAsync();
        }
    }
}