using Foundry.Domain.Interfaces;
using Foundry.Domain.Interfaces.Repositories;
using Foundry.Domain.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Foundry.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// A concrete and generic implementation of the IGenericRepository contract for state-based persistence
    /// using Entity Framework Core. It depends on the IApplicationDbContext abstraction, making it
    /// agnostic to the final DbContext implementation.
    /// </summary>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly IApplicationDbContext _context;

        public GenericRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <inheritdoc />
        public virtual async Task<TEntity?> GetByIdAsync(Guid id) => await _context.Set<TEntity>().FindAsync(id);

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync() => await _context.Set<TEntity>().ToListAsync();

        /// <inheritdoc />
        public virtual async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec) => await ApplySpecification(spec).FirstOrDefaultAsync();

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec) => await ApplySpecification(spec).ToListAsync();
        
        /// <inheritdoc />
        public virtual async Task<int> CountAsync(ISpecification<TEntity> spec) => await ApplySpecification(spec).CountAsync();
        
        /// <inheritdoc />
        public virtual async Task AddAsync(TEntity entity) => await _context.Set<TEntity>().AddAsync(entity);
        
        /// <inheritdoc />
        public virtual void Update(TEntity entity) => _context.Entry(entity).State = EntityState.Modified;

        /// <inheritdoc />
        public virtual void Remove(TEntity entity) => _context.Set<TEntity>().Remove(entity);

        /// <summary>
        /// A private helper method to apply a specification to the DbSet's IQueryable.
        /// </summary>
        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec);
        }
    }
}