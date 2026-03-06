using Microsoft.EntityFrameworkCore; // DbContext ve ilgili EF Core metotları için
using KutuphaneOtomasyonu.API.Data; // KutuphaneDbContext için
using KutuphaneOtomasyonu.API.Interfaces; // IRepository için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Repositories
{

    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly KutuphaneDbContext _context;

        public Repository(KutuphaneDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync(); 
        }

        public async Task<T> GetByIdAsync(int id)
        {

            return await _context.Set<T>().FindAsync(id);
        }
        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable();
        }


        public async Task<T> GetByIdAsync(string id)
        {
          
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; 
            return Task.CompletedTask; 
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id); 
            if (entity != null)
            {
                _context.Set<T>().Remove(entity); 
            }
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity); 
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id) != null;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id) != null;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync(); 
        }
    }
}