using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model.Auditing;

namespace Foundry.Infrastructure.Auditing.Implementations
{
    /// <summary>
    /// A Null Object pattern implementation of IAuditLogStore.
    /// It performs no operation, effectively disabling audit logging when registered.
    /// </summary>
    public class NullAuditLogStore : IAuditLogStore
    {
        /// <summary>
        /// Does nothing and completes immediately.
        /// </summary>
        public Task SaveAsync(IReadOnlyCollection<AuditLog> auditLogs, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}