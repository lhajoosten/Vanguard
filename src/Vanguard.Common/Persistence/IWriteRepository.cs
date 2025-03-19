using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Persistence
{
    /// <summary>
    /// Interface for writing entities to a repository
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier</typeparam>
    public interface IWriteRepository<T, TId> where T : IAggregateRoot<TId>
    {
        /// <summary>
        /// Adds an entity to the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>The added entity</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an entity in the repository
        /// </summary>
        /// <param name="entity">The entity to update</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>A task representing the update operation</returns>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity from the repository
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>A task representing the delete operation</returns>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
