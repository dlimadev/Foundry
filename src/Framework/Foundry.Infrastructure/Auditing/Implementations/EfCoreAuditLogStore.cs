using Foundry.Domain.Interfaces;
using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model.Auditing;

namespace Foundry.Infrastructure.Auditing.Implementations
{
    public class EfCoreAuditLogStore : IAuditLogStore
    {
        private readonly IApplicationDbContext _context;

        public EfCoreAuditLogStore(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(IReadOnlyCollection<AuditLog> auditLogs, CancellationToken cancellationToken = default)
        {
            if (auditLogs.Count == 0) return;

            await _context.Set<AuditLog>().AddRangeAsync(auditLogs, cancellationToken);
        }
    }
}