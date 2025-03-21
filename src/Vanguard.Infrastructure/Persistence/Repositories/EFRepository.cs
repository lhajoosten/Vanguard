using Microsoft.EntityFrameworkCore;
using Vanguard.Common.Abstractions;
using Vanguard.Common.Persistence;
using Vanguard.Infrastructure.Persistence.Context;

namespace Vanguard.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Generic repository implementation for Entity Framework
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root</typeparam>
    /// <typeparam name="TId">The type of the aggregate root's identifier</typeparam>
    public class EfRepository<T, TId> : IRepository<T, TId> where T : class, IAggregateRoot<TId>
    {
        protected readonly VanguardContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(VanguardContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        #region IReadRepository Implementation

        public virtual async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
        }

        #endregion

        #region IWriteRepository Implementation

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            // EntityFramework tracks entity state automatically
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Applies a specification to the DbSet
        /// </summary>
        protected virtual IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return spec.Apply(_dbSet);
        }
    }
}