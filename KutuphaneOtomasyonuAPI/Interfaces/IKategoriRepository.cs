// KutuphaneOtomasyonu.API.Interfaces/IKategoriRepository.cs
using KutuphaneOtomasyonu.API.Entities;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface IKategoriRepository : IRepository<Kategori>
    {

        Task<Kategori> GetCategoryByNameAsync(string categoryName);
     
    }
}