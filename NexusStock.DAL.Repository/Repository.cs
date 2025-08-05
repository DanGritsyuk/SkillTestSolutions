using Microsoft.EntityFrameworkCore;
using NexusStock.DAL.Repository.Contracts;
using System;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly NexusStockDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(NexusStockDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id) => 
            await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => 
            await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => 
            await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => 
            await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => 
            await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => 
            _context.Entry(entity).State = EntityState.Modified;

        public void Remove(T entity) => 
            _dbSet.Remove(entity);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => 
            await _dbSet.AnyAsync(predicate);
    }
}
