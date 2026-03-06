using System.Collections.Generic;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
 

        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id); 

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);
        Task DeleteAsync(string id);
      
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(string id); 

        Task SaveChangesAsync();
    }
}