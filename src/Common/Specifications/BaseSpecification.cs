using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescField)
            => OrderByDescending = orderByDescField;

        protected void AddOrderBy(Expression<Func<T, object>> orderByField)
            => OrderBy = orderByField;

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

    }
}
