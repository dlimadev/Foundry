using Confluent.Kafka;
using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model.Auditing;
using Foundry.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Foundry.Infrastructure.Auditing.Implementations
{
    /// <summary>
    /// An IAuditLogStore implementation that publishes audit logs to a Kafka topic.
    /// This is ideal for decoupled, high-throughput, centralized auditing systems.
    /// </summary>
    public class KafkaAuditLogStore : IAuditLogStore
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _auditTopic;

        public KafkaAuditLogStore(IProducer<string, string> producer, IOptions<AuditingSettings> auditingSettings)
        {
            _producer = producer;
            // The topic name is now read from the strongly-typed settings object.
            _auditTopic = auditingSettings.Value.Kafka?.TopicName ?? "default-audit-logs";
        }

        /// <inheritdoc />
        public async Task SaveAsync(IReadOnlyCollection<AuditLog> auditLogs, CancellationToken cancellationToken = default)
        {
            foreach (var log in auditLogs)
            {
                var message = new Message<string, string>
                {
                    // Using the entity's primary key from the audit log as the Kafka message key
                    // ensures all changes for the same entity go to the same partition, maintaining order.
                    Key = log.KeyValues,
                    Value = JsonSerializer.Serialize(log)
                };

                await _producer.ProduceAsync(_auditTopic, message, cancellationToken);
            }
        }
    }
}