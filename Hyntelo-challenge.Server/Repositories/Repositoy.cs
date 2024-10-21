using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hyntelo_challenge.Server.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<PaginatedResult<T>> GetAllAsync(int page, int pageSize);
        Task<PaginatedResult<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int page, int pageSize);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(BlogDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<PaginatedResult<T>> GetAllAsync(int page, int pageSize)
        {
            return await GetAllAsync(null, page, pageSize);
        }

        public async Task<PaginatedResult<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int page, int pageSize)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return;
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }
}
