//  DataAccess/Repositories/Repository.cs ---
using IMS_Group03.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS_Group03.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // All methods from AddAsync to RemoveRange are correct and unchanged.
        public virtual async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await _context.Set<T>().AddRangeAsync(entities);
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().Where(predicate).ToListAsync();
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public virtual async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public virtual void Remove(T entity) => _context.Set<T>().Remove(entity);
        public virtual void RemoveRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);
        public virtual void Update(T entity) => _context.Set<T>().Update(entity);

        
    }
}