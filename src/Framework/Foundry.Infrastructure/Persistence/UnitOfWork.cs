// The code should be in English
using Foundry.Domain.Interfaces;
using Foundry.Domain.Interfaces.Auditing;
using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Foundry.Domain.Model.Events;
using Foundry.Domain.Services;
using Foundry.Infrastructure.Auditing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Foundry.Infrastructure.Persistence
{
    /// <summary>
    /// The concrete implementation of the Unit of Work pattern.
    /// It orchestrates committing database changes for the main business transaction and
    /// subsequently handles cross-cutting concerns like auditing and dispatching domain events.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogStore _auditLogStore;
        private IDbContextTransaction? _currentTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        public UnitOfWork(
            DbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService,
            IAuditLogStore auditLogStore)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _auditLogStore = auditLogStore ?? throw new ArgumentNullException(nameof(auditLogStore));
        }

        /// <inheritdoc />
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            // --- 1. Prepare Side-Effects BEFORE Committing ---

            // A. Generate audit logs for all changed entities tracked by the DbContext.
            // This captures the "before" and "after" states while they are still available in the Change Tracker.
            var auditLogs = AuditHelper.CreateAuditLogs(_context, _currentUserService.UserId);

            // B. Add generic domain events (for cache invalidation, etc.) to the entities.
            AddDomainEventsForCacheInvalidation();

            // --- 2. Commit the MAIN Business Transaction ---
            // This saves the primary business data (e.g., the updated Stock) to the main database.
            // If this fails, an exception is thrown, and the side-effects (auditing/events) are never dispatched.
            var result = await _context.SaveChangesAsync(cancellationToken);

            // --- 3. Dispatch Side-Effects AFTER Successful Commit ---

            // C. Dispatch domain events (e.g., for cache invalidation) using MediatR.
            await DispatchDomainEventsAsync(cancellationToken);

            // D. Save the audit logs to their configured destination (separate DB, Kafka, File, etc.).
            // This happens *after* the main transaction to ensure we only audit what was successfully saved.
            if (auditLogs.Any())
            {
                await _auditLogStore.SaveAsync(auditLogs, cancellationToken);
            }

            return result;
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var entitiesWithEvents = _context.ChangeTracker.Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }

        private void AddDomainEventsForCacheInvalidation()
        {
            var changedCacheableEntities = _context.ChangeTracker.Entries<EntityBase>()
                .Where(e => e.Entity.GetType().IsDefined(typeof(CacheableAttribute), false) &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in changedCacheableEntities)
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.AddDomainEvent(new EntityCreatedEvent(entry.Entity));
                else if (entry.State == EntityState.Modified)
                    entry.Entity.AddDomainEvent(new EntityUpdatedEvent(entry.Entity));
                else if (entry.State == EntityState.Deleted)
                    entry.Entity.AddDomainEvent(new EntityDeletedEvent(entry.Entity));
            }
        }

        #region Explicit Transaction Management

        /// <inheritdoc />
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await CompleteAsync(cancellationToken);
                await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <inheritdoc />
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await (_currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
        #endregion
    }
}