using Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query().SingleOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Query().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await Query().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        // ---------------------------------------------
        //            🔥 Soft delete filtresi
        // ---------------------------------------------
        public IQueryable<T> Query()
        {
            var query = _context.Set<T>().AsQueryable();

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return query;
        }

        // ---------------------------------------------
        //  🔥 Soft delete + CreatedUserId filtreli Query
        // ---------------------------------------------
        public IQueryable<T> UserQuery(string userId)
        {
            var query = _context.Set<T>().AsQueryable();

            // Soft delete
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            // User-based filtre
            if (typeof(T).GetProperty("CreatedUserId") != null)
            {
                query = query.Where(e => EF.Property<string>(e, "CreatedUserId") == userId);
            }

            return query;
        }
    }
}
