using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Persistence
{
    /// <summary>
    /// Interface for reading entities from a repository
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier</typeparam>
    public interface IReadRepository<T, TId> where T : IAggregateRoot<TId>
    {
        /// <summary>
        /// Gets an entity by its identifier
        /// </summary>
        /// <param name="id">The identifier of the entity</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>The entity, or null if not found</returns>
        Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all entities
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>A read-only list of all entities</returns>
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists entities that satisfy a specification
        /// </summary>
        /// <param name="spec">The specification to apply</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>A read-only list of entities that satisfy the specification</returns>
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts entities that satisfy a specification
        /// </summary>
        /// <param name="spec">The specification to apply</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>The number of entities that satisfy the specification</returns>
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the first entity that satisfies a specification, or null if none do
        /// </summary>
        /// <param name="spec">The specification to apply</param>
        /// <param name="cancellationToken">A token for cancelling the operation</param>
        /// <returns>The first entity that satisfies the specification, or null</returns>
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
}
