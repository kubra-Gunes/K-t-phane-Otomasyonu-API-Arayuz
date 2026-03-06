// KutuphaneOtomasyonu.API.Repositories/YazarRepository.cs
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyonu.API.Data;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Repositories
{
    public class YazarRepository : Repository<Yazar>, IYazarRepository
    {
        public YazarRepository(KutuphaneDbContext context) : base(context)
        {
        }

        // IYazarRepository'deki özel metotların uygulanması
        public async Task<IEnumerable<Yazar>> GetAuthorsByNameAsync(string firstName, string lastName)
        {
            return await _context.Yazarlar
                                 .Where(y => y.Ad.Contains(firstName) && y.Soyad.Contains(lastName))
                                 .ToListAsync();
        }

        // Eğer Yazar için GetAllAsync metodunun varsayılan davranışını değiştirmek isterseniz override edebilirsiniz.
        // Örnek: Yazarla birlikte ilişkili kitapları da getirmek isterseniz:
        // public override async Task<IEnumerable<Yazar>> GetAllAsync()
        // {
        //     return await _context.Yazarlar.Include(y => y.Kitaplari).ToListAsync();
        // }
    }
}