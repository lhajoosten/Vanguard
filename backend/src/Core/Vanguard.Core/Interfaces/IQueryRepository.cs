using Ardalis.Specification;
using System.Linq.Expressions;
using Vanguard.Core.Base;

namespace Vanguard.Core.Interfaces
{
    /// <summary>
    /// Repository interface for read operations that leverages Ardalis.Specification
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IQueryRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
        // Add any additional query methods not covered by IReadRepositoryBase
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
