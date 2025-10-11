using Foundry.Domain.Interfaces;
using Foundry.Domain.Model;
using Foundry.Domain.Model.Attributes;
using Foundry.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sample.FinancialMarket.Domain.Aggregates.Portfolios;
using System.Reflection;

namespace Sample.FinancialMarket.Infrastructure.Persistence
{
    /// <summary>
    /// The concrete DbContext for our sample application, responsible for state-based (CRUD) persistence.
    /// It is marked as 'internal' to enforce the use of the IApplicationDbContext abstraction
    /// from the Foundry.Domain layer, preventing direct calls to SaveChangesAsync from outside the UoW.
    /// </summary>
    internal class FinanceDbContext : DbContext, IApplicationDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinanceDbContext"/> class.
        /// </summary>
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        // --- DbSets for CRUD Aggregates ---
        // Only entities that are persisted via state-persistence are listed here.
        // The 'Order' aggregate is NOT here because it is event-sourced via Marten.

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Bond> Bonds { get; set; }

        /// <summary>
        /// Overridden to apply all IEntityTypeConfiguration classes from the current assembly.
        /// This keeps the DbContext clean and organized.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Overridden to automatically dispatch domain events for cache invalidation
        /// before the changes are committed to the database.
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddDomainEventsForCacheInvalidation();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Scans the Change Tracker for entities marked with the [Cacheable] attribute
        /// that have been modified, and adds the appropriate domain event to their event collection.
        /// These events will be dispatched by the Unit of Work after the transaction is successfully committed.
        /// </summary>
        private void AddDomainEventsForCacheInvalidation()
        {
            var changedCacheableEntities = ChangeTracker.Entries<EntityBase>()
                .Where(e => e.Entity.GetType().IsDefined(typeof(CacheableAttribute), false) &&
                            (e.State == EntityState.Added ||
                             e.State == EntityState.Modified ||
                             e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in changedCacheableEntities)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.AddDomainEvent(new EntityCreatedEvent(entry.Entity));
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.AddDomainEvent(new EntityUpdatedEvent(entry.Entity));
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.Entity.AddDomainEvent(new EntityDeletedEvent(entry.Entity));
                }
            }
        }
    }
}