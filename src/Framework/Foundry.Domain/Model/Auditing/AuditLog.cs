namespace Foundry.Domain.Model.Auditing
{
    /// <summary>
    /// Represents a log entry for an audit trail, capturing changes to entities.
    /// </summary>
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string ActionType { get; set; } = string.Empty; // e.g., "Created", "Updated", "Deleted"
        public string EntityName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string KeyValues { get; set; } = string.Empty;   // JSON of the primary key(s)
        public string? OldValues { get; set; }  // JSON of the old state for updates
        public string? NewValues { get; set; }  // JSON of the new state for updates/creates
    }
}