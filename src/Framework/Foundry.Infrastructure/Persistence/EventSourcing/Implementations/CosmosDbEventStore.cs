using Foundry.Domain.Interfaces.EventSourcing;
using Foundry.Domain.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Foundry.Infrastructure.Persistence.EventSourcing.Implementations
{
    /// <summary>
    /// An implementation of IEventStore using Azure Cosmos DB.
    /// It stores events in one container and snapshots in another.
    /// The event container should be partitioned by '/aggregateId'.
    /// </summary>
    public class CosmosDbEventStore : IEventStore
    {
        private readonly Container _eventContainer;
        private readonly Container _snapshotContainer;

        /// <summary>
        /// We use Newtonsoft.Json's settings specifically for its TypeNameHandling feature,
        /// which is crucial for deserializing interfaces like IDomainEvent back to their concrete types.
        /// </summary>
        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbEventStore"/> class.
        /// </summary>
        public CosmosDbEventStore(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var eventContainerName = configuration["CosmosDb:EventContainerName"];
            var snapshotContainerName = configuration["CosmosDb:SnapshotContainerName"];

            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(eventContainerName) || string.IsNullOrEmpty(snapshotContainerName))
            {
                throw new ArgumentNullException("Cosmos DB configuration (DatabaseName, EventContainerName, SnapshotContainerName) must be provided in appsettings.");
            }

            _eventContainer = cosmosClient.GetContainer(databaseName, eventContainerName);
            _snapshotContainer = cosmosClient.GetContainer(databaseName, snapshotContainerName);
        }

        /// <inheritdoc />
        public async Task<List<IDomainEvent>> GetEventsForAggregateAsync(Guid aggregateId, int fromVersion)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.aggregateId = @aggregateId AND c.version > @fromVersion ORDER BY c.version ASC")
                .WithParameter("@aggregateId", aggregateId.ToString())
                .WithParameter("@fromVersion", fromVersion);

            var iterator = _eventContainer.GetItemQueryIterator<EventDocument>(query,
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(aggregateId.ToString()) });

            var domainEvents = new List<IDomainEvent>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var document in response)
                {
                    if (JsonConvert.DeserializeObject(document.Data, _serializerSettings) is IDomainEvent domainEvent)
                    {
                        domainEvents.Add(domainEvent);
                    }
                }
            }
            return domainEvents;
        }

        /// <inheritdoc />
        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
        {
            var partitionKey = new PartitionKey(aggregateId.ToString());
            var batch = _eventContainer.CreateTransactionalBatch(partitionKey);

            var currentVersion = expectedVersion;
            foreach (var domainEvent in events)
            {
                currentVersion++;
                var eventDocument = new EventDocument
                (
                    Id: Guid.NewGuid().ToString(),
                    AggregateId: aggregateId.ToString(),
                    Version: currentVersion,
                    EventType: domainEvent.GetType().Name,
                    Data: JsonConvert.SerializeObject(domainEvent, _serializerSettings),
                    Timestamp: DateTime.UtcNow
                );
                batch.CreateItem(eventDocument);
            }

            using var batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                // This could be due to a concurrency conflict or other transient error.
                throw new Exception($"Failed to save events for aggregate {aggregateId}. Status code: {batchResponse.StatusCode}. Reason: {batchResponse.ErrorMessage}");
            }
        }

        /// <inheritdoc />
        public async Task<IEventSourcedSnapshot?> GetLatestSnapshotAsync(Guid aggregateId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.aggregateId = @aggregateId ORDER BY c.version DESC OFFSET 0 LIMIT 1")
                .WithParameter("@aggregateId", aggregateId.ToString());

            var iterator = _snapshotContainer.GetItemQueryIterator<SnapshotDocument>(query,
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(aggregateId.ToString()) });

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                var document = response.FirstOrDefault();
                if (document != null)
                {
                    return JsonConvert.DeserializeObject(document.Data, _serializerSettings) as IEventSourcedSnapshot;
                }
            }
            return null;
        }

        /// <inheritdoc />
        public async Task SaveSnapshotAsync(IEventSourcedSnapshot snapshot)
        {
            var snapshotDocument = new SnapshotDocument(
                Id: $"{snapshot.AggregateId}:{snapshot.Version}",
                AggregateId: snapshot.AggregateId.ToString(),
                Version: snapshot.Version,
                SnapshotType: snapshot.GetType().Name,
                Data: JsonConvert.SerializeObject(snapshot, _serializerSettings)
            );

            await _snapshotContainer.UpsertItemAsync(snapshotDocument, new PartitionKey(snapshot.AggregateId.ToString()));
        }

        /// <summary>
        /// Private record representing the schema for a document in the 'events' container.
        /// </summary>
        private record EventDocument(string Id, string AggregateId, int Version, string EventType, string Data, DateTime Timestamp);

        /// <summary>
        /// Private record representing the schema for a document in the 'snapshots' container.
        /// </summary>
        private record SnapshotDocument(string Id, string AggregateId, int Version, string SnapshotType, string Data);
    }
}