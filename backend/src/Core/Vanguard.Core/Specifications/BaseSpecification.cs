using Ardalis.Specification;
using System.Linq.Expressions;

namespace Vanguard.Core.Specifications
{
    /// <summary>
    /// Base class for specifications that extends Ardalis.Specification
    /// </summary>
    /// <typeparam name="T">The type of entity that the specification is for</typeparam>
    public abstract class BaseSpecification<T> : Specification<T>
    {
        protected void ApplyFilterIfNotNull<TProperty>(TProperty value, Expression<Func<T, bool>> predicate)
            where TProperty : class
        {
            if (value != null)
            {
                Query.Where(predicate);
            }
        }

        protected void ApplyFilterIfNotDefault<TProperty>(TProperty value, Expression<Func<T, bool>> predicate)
            where TProperty : struct
        {
            if (!EqualityComparer<TProperty>.Default.Equals(value, default))
            {
                Query.Where(predicate);
            }
        }
    }
}