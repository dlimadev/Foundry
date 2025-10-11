using Foundry.Domain.Model.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Foundry.Infrastructure.Auditing
{
    public static class AuditHelper
    {
        public static List<AuditLog> CreateAuditLogs(DbContext context, string? userId)
        {
            var auditEntries = new List<AuditLog>();
            context.ChangeTracker.DetectChanges();
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is not AuditLog &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    EntityName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    ActionType = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    KeyValues = JsonSerializer.Serialize(entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue))
                };

                var oldValues = entry.State == EntityState.Deleted || entry.State == EntityState.Modified
                    ? entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p])
                    : null;

                var newValues = entry.State == EntityState.Added || entry.State == EntityState.Modified
                    ? entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p])
                    : null;

                if (entry.State == EntityState.Modified && !entry.Properties.Any(p => p.IsModified))
                {
                    continue; 
                }

                auditLog.OldValues = oldValues == null ? null : JsonSerializer.Serialize(oldValues);
                auditLog.NewValues = newValues == null ? null : JsonSerializer.Serialize(newValues);

                auditEntries.Add(auditLog);
            }
            return auditEntries;
        }
    }
}