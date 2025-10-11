using System.Linq.Expressions;

namespace Foundry.Domain.Interfaces.Specifications
{
    /// <summary>
    /// An abstract base class for concrete specifications, providing common functionality
    /// to reduce boilerplate code when creating new specifications.
    /// </summary>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        /// <inheritdoc />
        public Expression<Func<T, bool>> Criteria { get; }
        
        /// <inheritdoc />
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        
        /// <inheritdoc />
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        
        /// <inheritdoc />
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        /// <summary>
        /// Protected constructor for specifications.
        /// </summary>
        /// <param name="criteria">The filtering criteria for this specification.</param>
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Adds an include expression to the specification for eager loading related data.
        /// </summary>
        /// <param name="includeExpression">The expression representing the navigation property to include.</param>
        protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);

        /// <summary>
        /// Applies an ascending ordering to the specification.
        /// </summary>
        /// <param name="orderByExpression">The expression representing the property to order by.</param>
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;

        /// <summary>
        /// Applies a descending ordering to the specification.
        /// </summary>
        /// <param name="orderByDescendingExpression">The expression representing the property to order by descending.</param>
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) => OrderByDescending = orderByDescendingExpression;
    }
}