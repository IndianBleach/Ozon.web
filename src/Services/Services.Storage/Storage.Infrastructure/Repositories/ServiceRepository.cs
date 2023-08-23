using Common.DataQueries;
using Common.Repositories;
using Common.Specifications;
using Products.Data.Context;
using Products.Data.Entities;

namespace Storage.Infrastructure.Repositories
{
    public class ServiceRepository<T> : IServiceRepository<T> where T : TServiceEntity
    {
        private readonly ApplicationContext _dbContext;

        public ServiceRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Any(Func<T, bool> predicate)
        {
            return _dbContext.Set<T>().Any(predicate);
        }

        public QueryResult<T> Create(T entity)
        {
            try
            {
                _dbContext.Add(entity);

                _dbContext.SaveChanges();

                return QueryResult<T>.Successed(entity);
            }
            catch (Exception exp)
            {
                return QueryResult<T>.Failure(exp.Message);
            }
        }

        public IEnumerable<T> Find(ISpecification<T> specification)
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
