using KutuphaneOtomasyonu.API.Entities; // Kitap entity'si için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{

    public interface IKitapRepository : IRepository<Kitap>
    {

        Task<IEnumerable<Kitap>> GetBooksByAuthorIdAsync(int authorId);


        Task<IEnumerable<Kitap>> GetBooksByCategoryNameAsync(string categoryName);


        Task<bool> UpdateAsync(Kitap kitap);
    }
}