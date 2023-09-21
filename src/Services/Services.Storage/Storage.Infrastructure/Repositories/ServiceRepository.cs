using Common.DataQueries;
using Common.Repositories;
using Common.Specifications;
using Microsoft.EntityFrameworkCore.Storage;
using Storage.Data.Context;
using Storage.Data.Entities;

namespace Storage.Infrastructure.Repositories
{
    public class ServiceRepository<T> : IServiceRepository<T> where T : TServiceEntity
    {
        private readonly ApplicationContext _dbContext;

        public ServiceRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int AddRange(T[] entities)
        {
            _dbContext.Set<T>().AddRange(entities);

            return 0;
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

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        public IDbContextTransaction NewTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }
    }
}
