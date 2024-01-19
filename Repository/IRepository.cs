using System.Linq.Expressions;

namespace APICatalogo.Repository
{
    public interface IRepository<Type> where Type : class
    {
        IQueryable<Type?> Get();
        Type? GetById(Expression<Func<Type?, bool>> predicate);
        void Add(Type entity);
        void Update(Type entity);
        void Delete(Type entity);
    }
}
