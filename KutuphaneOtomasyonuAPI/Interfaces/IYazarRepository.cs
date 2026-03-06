// KutuphaneOtomasyonu.API.Interfaces/IYazarRepository.cs
using KutuphaneOtomasyonu.API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface IYazarRepository : IRepository<Yazar>
    {
        // Yazar'a özel metotlar buraya eklenebilir.
        // Örnek: Belirli bir isme göre yazarları bulma
        Task<IEnumerable<Yazar>> GetAuthorsByNameAsync(string firstName, string lastName);
    }
}