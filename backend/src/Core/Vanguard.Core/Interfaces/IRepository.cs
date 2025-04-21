using Ardalis.Specification;
using Vanguard.Core.Base;

namespace Vanguard.Core.Interfaces
{
    /// <summary>
    /// Repository interface that extends the Ardalis IRepositoryBase interface
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
        // Add any additional methods not covered by IRepositoryBase
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
    }
}
