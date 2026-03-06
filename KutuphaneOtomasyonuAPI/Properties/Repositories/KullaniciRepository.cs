// KutuphaneOtomasyonu.API.Repositories/KullaniciRepository.cs
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyonu.API.Data;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Repositories
{
    public class KullaniciRepository : Repository<Kullanici>, IKullaniciRepository
    {
        public KullaniciRepository(KutuphaneDbContext context) : base(context)
        {
        }

        // IKullaniciRepository'deki özel metotların uygulanması
        public async Task<Kullanici> GetUserByEmailAsync(string email)
        {
            return await _context.Kullanicilar.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<Kullanici> GetByEmailAsync(string email)
        {
            // Veritabanında belirtilen e-posta adresine sahip kullanıcıyı bul
            return await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Email == email);
        }
        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Kullanicilar.AnyAsync(u => u.Email == email);
        }

        // Eğer kullanıcı ile ödünç kayıtlarını da getirmek isterseniz GetAllAsync'i override edebilirsiniz.
        // public override async Task<IEnumerable<Kullanici>> GetAllAsync()
        // {
        //     return await _context.Kullanicilar.Include(u => u.OduncKayitlari).ToListAsync();
        // }
    }
}