using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Persistence
{
    /// <summary>
    /// Interface for a combined read/write repository for entities
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier</typeparam>
    public interface IRepository<T, TId> : IReadRepository<T, TId>, IWriteRepository<T, TId>
       where T : IAggregateRoot<TId>
    {
    }
}
