using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Persistence
{
    /// <summary>
    /// Interface for the specification pattern
    /// </summary>
    /// <typeparam name="T">The type to which the specification applies</typeparam>
    public interface ISpecification<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Checks if an entity satisfies the specification
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>True if the entity satisfies the specification, false otherwise</returns>
        bool IsSatisfiedBy(T entity);

        /// <summary>
        /// Applies the specification to a query
        /// </summary>
        /// <param name="query">The query to which the specification is applied</param>
        /// <returns>A modified query that includes the specification</returns>
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
