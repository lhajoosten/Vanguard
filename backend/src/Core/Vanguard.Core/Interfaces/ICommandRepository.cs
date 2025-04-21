using Vanguard.Core.Base;

namespace Vanguard.Core.Interfaces
{
    /// <summary>
    /// Repository interface for write operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface ICommandRepository<T> where T : class, IAggregateRoot
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
