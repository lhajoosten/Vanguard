using Ardalis.Specification;
using System.Linq.Expressions;

namespace Vanguard.Core.Specifications
{
    /// <summary>
    /// Factory class for creating reusable specifications
    /// </summary>
    public static class SpecificationFactory
    {
        /// <summary>
        /// Creates a simple specification with a criteria.
        /// </summary>
        public static Specification<T> Create<T>(Expression<Func<T, bool>> criteria) where T : class
        {
            return new GenericSpecification<T>(criteria);
        }

        /// <summary>
        /// Creates a specification with criteria and includes.
        /// </summary>
        public static Specification<T> Create<T>(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes) where T : class
        {
            return new GenericSpecification<T>(criteria, includes);
        }

        private class GenericSpecification<T> : Specification<T> where T : class
        {
            public GenericSpecification(Expression<Func<T, bool>> criteria)
            {
                Query.Where(criteria);
            }

            public GenericSpecification(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
            {
                Query.Where(criteria);
                foreach (var include in includes)
                {
                    Query.Include(include);
                }
            }
        }
    }
}
