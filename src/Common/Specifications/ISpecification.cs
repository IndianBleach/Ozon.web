using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Specifications
{
    public interface ISpecification<T> where T : class
    {
        Expression<Func<T, bool>> Criteria { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        Expression<Func<T, object>> OrderBy { get; }

        Expression<Func<T, object>> OrderByDescending { get; }
    }
}
