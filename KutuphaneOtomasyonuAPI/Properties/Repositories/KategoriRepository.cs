// KutuphaneOtomasyonu.API.Repositories/KategoriRepository.cs
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyonu.API.Data;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Repositories
{
    public class KategoriRepository : Repository<Kategori>, IKategoriRepository
    {
        public KategoriRepository(KutuphaneDbContext context) : base(context)
        {
        }

        // IKategoriRepository'deki özel metotların uygulanması
        public async Task<Kategori> GetCategoryByNameAsync(string categoryName)
        {
            return await _context.Kategoriler
                                 .FirstOrDefaultAsync(c => c.KategoriAdi == categoryName);
        }

        // IKategoriRepository.Update metot uygulaması
        public void Update(Kategori entity)
        {
            _context.Set<Kategori>().Update(entity); // veya _context.Entry(entity).State = EntityState.Modified;
        }

        // Eğer kategori ile birlikte kitaplarını da getirmek isterseniz GetAllAsync'i override edebilirsiniz.
        // public override async Task<IEnumerable<Kategori>> GetAllAsync()
        // {
        //     return await _context.Kategoriler.Include(c => c.Kitaplari).ToListAsync();
        // }
    }
}