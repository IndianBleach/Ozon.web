using Common.DataQueries;
using Common.Repositories;
using Common.Specifications;
using Microsoft.EntityFrameworkCore.Storage;
using Storage.Data.Context;
using Storage.Data.Entities;

namespace Storage.Infrastructure.Repositories
{
    public class StorageRepository<T> : IServiceRepository<T>, IServiceAsyncRepository<T> where T : TServiceEntity
    {
        private readonly StorageDbContext _dbContext;

        public StorageRepository(StorageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int AddRange(T[] entities)
        {
            _dbContext.Set<T>().AddRange(entities);

            return _dbContext.SaveChanges();
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

        public async Task<QueryResult<T>> CreateAsync(T entity)
        {
            await _dbContext.AddAsync<T>(entity);
            var r = await _dbContext.SaveChangesAsync();

            return  r > 0 ? QueryResult<T>.Successed(entity) : QueryResult<T>.Failure("Something went wrong");
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
