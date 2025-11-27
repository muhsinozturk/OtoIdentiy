 using System.Linq.Expressions;

namespace Application.Abstractions.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);

        IQueryable<T> Query();               // Soft delete’siz normal query
        IQueryable<T> UserQuery(string userId);  // Soft delete + CreatedUserId filtresi
    }
}

