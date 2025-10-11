using Foundry.Domain.Interfaces.Specifications;

namespace Foundry.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Defines the generic repository contract, designed to work with the Specification pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of the Aggregate Root this repository manages.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> spec);
        Task<IReadOnlyList<TEntity>> ListAllAsync();
        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);
        Task<int> CountAsync(ISpecification<TEntity> spec);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}