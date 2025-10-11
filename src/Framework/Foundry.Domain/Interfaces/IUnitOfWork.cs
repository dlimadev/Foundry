namespace Foundry.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for the Unit of Work pattern. Its sole responsibility
    /// is to commit the business transaction atomically.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves all tracked changes to the underlying data store and dispatches domain events.
        /// </summary>
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction explicitly.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the active database transaction.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the active database transaction.
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}