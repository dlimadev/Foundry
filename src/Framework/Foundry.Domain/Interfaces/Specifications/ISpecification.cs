using System.Linq.Expressions;

namespace Foundry.Domain.Interfaces.Specifications
{
    /// <summary>
    /// Defines the contract for the Specification Pattern.
    /// It encapsulates a query's logic (filtering, ordering, and includes) in a single object.
    /// </summary>
    /// <typeparam name="T">The type of the entity to which the specification is applied.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// The filtering criteria (the 'Where' clause).
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// A list of navigation properties to be eager-loaded (the 'Include' clauses).
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// The ordering expression (the 'OrderBy' clause).
        /// </summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>
        /// The descending ordering expression (the 'OrderByDescending' clause).
        /// </summary>
        Expression<Func<T, object>>? OrderByDescending { get; }
    }
}