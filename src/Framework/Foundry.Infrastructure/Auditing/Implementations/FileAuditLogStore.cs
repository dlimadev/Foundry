using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model.Auditing;
using Foundry.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Foundry.Infrastructure.Auditing.Implementations
{
    /// <summary>
    /// An IAuditLogStore implementation that appends audit logs as JSON to a local text file.
    /// This implementation is thread-safe.
    /// </summary>
    public class FileAuditLogStore : IAuditLogStore
    {
        private readonly string _filePath;

        // A semaphore is used to ensure that only one thread writes to the file at a time.
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public FileAuditLogStore(IOptions<AuditingSettings> auditingSettings)
        {
            // The file path is read from the strongly-typed settings object.
            _filePath = auditingSettings.Value.File?.FilePath ?? "audit.log";
        }

        /// <inheritdoc />
        public async Task SaveAsync(IReadOnlyCollection<AuditLog> auditLogs, CancellationToken cancellationToken = default)
        {
            if (auditLogs == null || auditLogs.Count == 0) return;

            var options = new JsonSerializerOptions { WriteIndented = true };
            var logContent = JsonSerializer.Serialize(auditLogs, options);

            // Ensure thread-safe file access.
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Ensure the directory exists.
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.AppendAllTextAsync(_filePath, logContent, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}