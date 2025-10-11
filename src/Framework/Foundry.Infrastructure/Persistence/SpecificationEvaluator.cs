using Foundry.Domain.Interfaces.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Foundry.Infrastructure.Persistence
{
    /// <summary>
    /// A utility class that applies an ISpecification to an IQueryable data source.
    /// </summary>
    public static class SpecificationEvaluator<TEntity> where TEntity : class
    {
        /// <summary>
        /// Applies the criteria, includes, and ordering from a specification to an IQueryable.
        /// </summary>
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null) query = query.Where(spec.Criteria);
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            if (spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null) query = query.OrderByDescending(spec.OrderByDescending);
            return query;
        }
    }
}