using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Foundry.Domain.Interfaces
{
    /// <summary>
    /// Defines the limited contract for the application's DbContext that is exposed to other layers.
    /// It purposefully HIDES SaveChangesAsync to enforce the use of IUnitOfWork.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Creates a DbSet<TEntity> that can be used to query and save instances of TEntity.
        /// </summary>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Provides access to the EF Core Change Tracker for inspecting entity states.
        /// </summary>
        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// Provides access to the entity entry for a given entity instance.
        /// </summary>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    }
}