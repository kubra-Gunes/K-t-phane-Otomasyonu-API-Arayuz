// KutuphaneOtomasyonu.API.Interfaces/IKullaniciRepository.cs
using KutuphaneOtomasyonu.API.Entities;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Interfaces
{
    public interface IKullaniciRepository : IRepository<Kullanici>
    {
        Task<Kullanici> GetUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);

        Task<Kullanici> GetByEmailAsync(string email);
    }
}