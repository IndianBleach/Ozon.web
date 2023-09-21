using Common.DataQueries;
using Common.Specifications;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IServiceRepository<T> where T : class
    {
        int AddRange(T[] entities);

        QueryResult<T> Create(T entity);

        T? FirstOrDefault(ISpecification<T> spec);

        IEnumerable<T> Find(ISpecification<T> specification);

        bool Any(Func<T, bool> predicate);

        IEnumerable<T> GetAll();

        IDbContextTransaction NewTransaction();
    }
}
