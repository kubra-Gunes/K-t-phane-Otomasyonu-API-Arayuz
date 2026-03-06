using Microsoft.EntityFrameworkCore; // Include metodu için
using KutuphaneOtomasyonu.API.Data; // KutuphaneDbContext için
using KutuphaneOtomasyonu.API.Entities; // Kitap entity'si için
using KutuphaneOtomasyonu.API.Interfaces; // IKitapRepository için
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Repositories
{
    
    public class KitapRepository : Repository<Kitap>, IKitapRepository
    {
       
        public KitapRepository(KutuphaneDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Kitap>> GetBooksByAuthorIdAsync(int authorId)
        {
            return await _context.Kitaplar 
                                 .Where(k => k.YazarId == authorId) 
                                 .Include(k => k.Yazar)
                                 .Include(k => k.Kategori)
                                 .ToListAsync(); 
        }

        public async Task<IEnumerable<Kitap>> GetBooksByCategoryNameAsync(string categoryName)
        {
            return await _context.Kitaplar
                                 .Include(k => k.Kategori)
                                 .Where(k => k.Kategori.KategoriAdi == categoryName)
                                 .Include(k => k.Yazar)
                                 .ToListAsync();
        }

     
        public override async Task<IEnumerable<Kitap>> GetAllAsync()
        {
            return await _context.Kitaplar
                                 .Include(k => k.Yazar)
                                 .Include(k => k.Kategori)
                                 .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Kitap kitap)
        {
            var existingKitap = await _context.Kitaplar.FindAsync(kitap.KitapId);
            if (existingKitap == null)
            {
                return false; 
            }


            _context.Entry(existingKitap).CurrentValues.SetValues(kitap);

            return await _context.SaveChangesAsync() > 0; // Değişiklikler kaydedilirse true döner
        }
    }
}