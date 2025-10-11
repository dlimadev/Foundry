using Foundry.Domain.Model.Auditing;

namespace Foundry.Domain.Interfaces.Auditing
{
    /// <summary>
    /// Defines the contract for an audit log storage strategy.
    /// This allows the auditing destination to be pluggable (e.g., database, message queue, console).
    /// </summary>
    public interface IAuditLogStore
    {
        /// <summary>
        /// Saves a collection of audit logs to the configured destination.
        /// </summary>
        Task SaveAsync(IReadOnlyCollection<AuditLog> auditLogs, CancellationToken cancellationToken = default);
    }
}