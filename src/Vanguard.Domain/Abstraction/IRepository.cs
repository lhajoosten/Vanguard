using Vanguard.Domain.Base;

namespace Vanguard.Domain.Abstraction
{
    public interface IRepository<TEntity, TId> where TEntity : AggregateRoot<TId>
    {
        Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    }
}
