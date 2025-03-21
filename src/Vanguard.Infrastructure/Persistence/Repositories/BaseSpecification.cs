using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Vanguard.Common.Abstractions;
using Vanguard.Common.Persistence;

namespace Vanguard.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Base class for specifications
    /// </summary>
    /// <typeparam name="T">The type of entity that the specification applies to</typeparam>
    public abstract class BaseSpecification<T> : ISpecification<T> where T : class, IAggregateRoot
    {
        /// <summary>
        /// Gets the criteria expression
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; private set; }

        /// <summary>
        /// Gets the include expressions
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = [];

        /// <summary>
        /// Gets the include strings
        /// </summary>
        public List<string> IncludeStrings { get; } = [];

        /// <summary>
        /// Gets the order by expressions
        /// </summary>
        public List<(Expression<Func<T, object>> KeySelector, bool IsDescending)> OrderByExpressions { get; } = [];

        /// <summary>
        /// Gets the group by expression
        /// </summary>
        public Expression<Func<T, object>> GroupBy { get; private set; }

        /// <summary>
        /// Gets the number of items to skip
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// Gets the number of items to take
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// Gets a value indicating whether paging is enabled
        /// </summary>
        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class
        /// </summary>
        protected BaseSpecification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class with a criteria expression
        /// </summary>
        /// <param name="criteria">The criteria expression</param>
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Checks if an entity satisfies the specification
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>True if the entity satisfies the specification, false otherwise</returns>
        public virtual bool IsSatisfiedBy(T entity)
        {
            if (Criteria == null)
                return true;

            var predicate = Criteria.Compile();
            return predicate(entity);
        }

        /// <summary>
        /// Applies the specification to a query
        /// </summary>
        /// <param name="query">The query to which the specification is applied</param>
        /// <returns>The query with the specification applied</returns>
        public virtual IQueryable<T> Apply(IQueryable<T> query)
        {
            // Apply criteria
            if (Criteria != null)
            {
                query = query.Where(Criteria);
            }

            // Apply includes
            query = Includes.Aggregate(query, (current, include) => current.Include(include));
            query = IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (OrderByExpressions.Count != 0)
            {
                var (KeySelector, IsDescending) = OrderByExpressions.First();
                var orderedQuery = IsDescending
                    ? query.OrderByDescending(KeySelector)
                    : query.OrderBy(KeySelector);

                foreach (var orderExpression in OrderByExpressions.Skip(1))
                {
                    orderedQuery = orderExpression.IsDescending
                        ? orderedQuery.ThenByDescending(orderExpression.KeySelector)
                        : orderedQuery.ThenBy(orderExpression.KeySelector);
                }

                query = orderedQuery;
            }

            // Apply grouping
            if (GroupBy != null)
            {
                query = query.GroupBy(GroupBy).SelectMany(x => x);
            }

            // Apply paging
            if (IsPagingEnabled)
            {
                query = query.Skip(Skip).Take(Take);
            }

            return query;
        }

        /// <summary>
        /// Adds criteria to the specification
        /// </summary>
        /// <param name="criteria">The criteria expression</param>
        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Adds an include expression
        /// </summary>
        /// <param name="includeExpression">The include expression</param>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Adds an include string
        /// </summary>
        /// <param name="includeString">The include string</param>
        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        /// <summary>
        /// Adds an order by expression
        /// </summary>
        /// <param name="orderByExpression">The order by expression</param>
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderByExpressions.Add((orderByExpression, false));
        }

        /// <summary>
        /// Adds an order by descending expression
        /// </summary>
        /// <param name="orderByDescendingExpression">The order by descending expression</param>
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByExpressions.Add((orderByDescendingExpression, true));
        }

        /// <summary>
        /// Adds a group by expression
        /// </summary>
        /// <param name="groupByExpression">The group by expression</param>
        protected void AddGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        /// <summary>
        /// Applies paging to the specification
        /// </summary>
        /// <param name="skip">The number of items to skip</param>
        /// <param name="take">The number of items to take</param>
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// Disables paging
        /// </summary>
        protected void DisablePaging()
        {
            IsPagingEnabled = false;
        }
    }
}
