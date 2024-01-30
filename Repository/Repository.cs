using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICatalogo.Repository
{
    public class Repository<Type> : IRepository<Type> where Type : class
    {
        protected readonly ApiCatalogoContext _dbContext;

        public Repository(ApiCatalogoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Type entity)
        {
            _dbContext.Set<Type>().Add(entity);
        }

        public void Delete(Type entity)
        {
            _dbContext.Set<Type>().Remove(entity);
        }

        public IQueryable<Type?> Get()
        {
            return _dbContext.Set<Type>().AsNoTracking();
        }
           
        public async Task<Type?> GetById(Expression<Func<Type?, bool>> predicate)
        {
            return await _dbContext.Set<Type>().SingleOrDefaultAsync(predicate);
        }

        public void Update(Type entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<Type>().Update(entity);
        }
    }
}
