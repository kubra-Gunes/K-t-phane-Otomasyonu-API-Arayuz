// KutuphaneOtomasyonu.API.Repositories/OduncKayitRepository.cs
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyonu.API.Data;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // DateTime.Now için

namespace KutuphaneOtomasyonu.API.Repositories
{
    public class OduncKayitRepository : Repository<OduncKayit>, IOduncKayitRepository
    {
        public OduncKayitRepository(KutuphaneDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OduncKayit>> GetBorrowRecordsByUserIdAsync(int userId)
        {
            return await _context.OduncKayitlari
                                 .Where(oc => oc.KullaniciId == userId)
                                 .Include(oc => oc.Kitap)      
                                 .Include(oc => oc.Kullanici)  
                                 .ToListAsync();
        }

        public async Task<IEnumerable<OduncKayit>> GetOverdueBorrowRecordsAsync()
        {
            return await _context.OduncKayitlari
                                 .Where(oc => oc.Durum == "OduncAlindi" && oc.SonTeslimTarihi < DateTime.Now)
                                 .Include(oc => oc.Kitap)
                                 .Include(oc => oc.Kullanici)
                                 .ToListAsync();
        }

        // Ödünç Kayıtları için GetAllAsync metodunu override ederek ilgili verileri de yükleyebiliriz
        public override async Task<IEnumerable<OduncKayit>> GetAllAsync()
        {
            return await _context.OduncKayitlari
                                 .Include(oc => oc.Kitap)
                                 .Include(oc => oc.Kullanici)
                                 .ToListAsync();
        }
    }
}