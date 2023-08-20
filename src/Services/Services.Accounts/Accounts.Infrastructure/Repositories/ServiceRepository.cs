using Common.Repositories;
using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Data.Context;

namespace Accounts.Infrastructure.Repositories
{
    internal class ServiceRepository<T> : IServiceRepository<T> where T : TEntity
    {
        private readonly ApplicationContext _dbContext;

        public ServiceRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<T> FindBy(ISpecification<T> specification)
        {
            return SpecificationBuilder<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
        }

        public T? FirstOrDefault(ISpecification<T> spec)
        {
            return SpecificationBuilder<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec)
                .FirstOrDefault();
        }
    }
}
