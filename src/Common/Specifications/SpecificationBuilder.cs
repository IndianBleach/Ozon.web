using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Specifications
{
    public class SpecificationBuilder<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            IQueryable<T> buildQuery = inputQuery;
            if (spec.Criteria != null)
            {
                buildQuery = buildQuery.Where(spec.Criteria);
            }
            if (spec.OrderBy != null)
            {
                buildQuery = buildQuery.OrderBy(spec.OrderBy);
            }
            if (spec.OrderByDescending != null)
            {
                buildQuery = buildQuery.OrderByDescending(spec.OrderByDescending);
            }

            buildQuery = spec.Includes.Aggregate(buildQuery, (current, include) => current.Include(include));

            return buildQuery;
        }
    }
}
