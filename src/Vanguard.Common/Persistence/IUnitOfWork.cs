namespace Vanguard.Common.Persistence
{
    /// <summary>
    /// Interface for a unit of work, which groups multiple operations into a single transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves all changes made in this unit of work to the database
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>The number of state entries written to the database</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all changes and dispatches domain events
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>True if the operation was successful, false otherwise</returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
