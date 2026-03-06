using System; // DateTime için
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq; // .Any() ve .Select() için
using System.Threading.Tasks;
using KutuphaneOtomasyonu.API.Controllers.KutuphaneOtomasyonu.API.Models;
using KutuphaneOtomasyonu.API.Data;
using KutuphaneOtomasyonu.API.Entities; // OduncKayit, Kitap, Kullanici Entity'leri için
using KutuphaneOtomasyonu.API.Interfaces; // Repository'ler için
// using KutuphaneOtomasyonu.API.Models; // OduncAlmaByEmailInputDto buraya taşındığı için bu using'e gerek kalmayabilir
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // .Include() için

namespace KutuphaneOtomasyonu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OduncIslemleriController : ControllerBase
    {
        private readonly IOduncKayitRepository _oduncKayitRepository;
        private readonly IKitapRepository _kitapRepository;
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly KutuphaneDbContext _context;

        public OduncIslemleriController(
            IOduncKayitRepository oduncKayitRepository,
            IKitapRepository kitapRepository,
            IKullaniciRepository kullaniciRepository,
            KutuphaneDbContext context)
        {
            _oduncKayitRepository = oduncKayitRepository;
            _kitapRepository = kitapRepository;
            _kullaniciRepository = kullaniciRepository;
            _context = context;
        }

        // GET: api/OduncIslemleri - Tüm ödünç kayıtlarını getirir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OduncKayit>>> GetOduncKayitlari()
        {
            var kayitlar = await _oduncKayitRepository.GetAllAsync();
            return Ok(kayitlar);
        }

        // GET: api/OduncIslemleri/5 - Belirli bir ödünç kaydını ID'ye göre getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<OduncKayit>> GetOduncKayit(int id)
        {
            var kayit = await _oduncKayitRepository.GetByIdAsync(id);

            if (kayit == null)
            {
                return NotFound();
            }

            return Ok(kayit);
        }

        // GET: api/OduncIslemleri/byUser/1 - Belirli bir kullanıcıya ait ödünç kayıtlarını getirir
        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<IEnumerable<KutuphaneOtomasyonu.API.Models.OduncIslemDto>>> GetBorrowRecordsByUser(int userId) // Dönüş tipini OduncIslemDto olarak değiştiriyoruz
        {
            var records = await _context.OduncKayitlari
                                        .Include(o => o.Kitap)     // Kitap bilgilerini yükle
                                        .Include(o => o.Kullanici) // Kullanıcı bilgilerini yükle
                                        .Where(o => o.KullaniciId == userId)
                                        .Select(o => new KutuphaneOtomasyonu.API.Models.OduncIslemDto // OduncIslemDto'ya dönüştür
                                        {
                                            OduncIslemId = o.OduncId,
                                            KullaniciId = o.KullaniciId,
                                            KullaniciEmail = o.Kullanici.Email,
                                            KitapId = o.KitapId,
                                            KitapAdi = o.Kitap.KitapAdi,
                                            OduncAlmaTarihi = o.OduncAlmaTarihi,
                                            SonTeslimTarihi = o.SonTeslimTarihi,
                                            TeslimEdildi = (o.Durum == "IadeEdildi"),
                                            TeslimTarihi = o.IadeTarihi
                                        })
                                        .ToListAsync();

            if (records == null || !records.Any())
            {
                return NotFound($"Kullanıcı ID'si {userId} olan ödünç kayıt bulunamadı.");
            }
            return Ok(records);
        }

        //// GET: api/OduncIslemleri/overdue - Gecikmiş ödünç kayıtlarını getirir
        //[HttpGet("overdue")]
        //public async Task<ActionResult<IEnumerable<OduncKayit>>> GetOverdueBorrowRecords()
        //{
        //    // Bu metodu kullanabilmek için IOduncKayitRepository'nize bu metodu eklemeniz gerekir.
        //    // Örneğin: Task<IEnumerable<OduncKayit>> GetOverdueBorrowRecordsAsync();
        //    // Eğer eklemediyseniz, _context üzerinden aşağıdaki gibi sorgulayabilirsiniz:
        //    var overdueRecords = await _context.OduncKayitlari
        //                                       .Where(o => o.Durum != "IadeEdildi" && o.SonTeslimTarihi < DateTime.Now)

        //                                       .ToListAsync();

        //    if (overdueRecords == null || !overdueRecords.Any())
        //    {
        //        return NotFound("Gecikmiş ödünç kayıt bulunamadı.");
        //    }
        //    return Ok(overdueRecords);
        //}



        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<OduncIslemDto>>> GetOverdueBorrowRecords()
        {
            var overdueRecords = await _context.OduncKayitlari
                                               .Include(o => o.Kitap)
                                               .Include(o => o.Kullanici)
                                               .Where(o => o.Durum != "IadeEdildi" && o.SonTeslimTarihi < DateTime.Now)
                                               .Select(o => new OduncIslemDto
                                               {
                                                   OduncIslemId = o.OduncId,
                                                   KullaniciId = o.KullaniciId,
                                                   KullaniciEmail = o.Kullanici.Email,
                                                   KitapId = o.KitapId,
                                                   KitapAdi = o.Kitap.KitapAdi,
                                                   OduncAlmaTarihi = o.OduncAlmaTarihi,
                                                   SonTeslimTarihi = o.SonTeslimTarihi,
                                                   TeslimEdildi = (o.Durum == "IadeEdildi"),
                                                   TeslimTarihi = o.IadeTarihi
                                               })
                                               .ToListAsync();

            if (overdueRecords == null || !overdueRecords.Any())
            {
                return NotFound("Gecikmiş ödünç kayıt bulunamadı.");
            }
            return Ok(overdueRecords);
        }




        // GET: api/OduncIslemleri/tumOduncIslemleri - Tüm ödünç işlemlerini (iade edilmiş veya edilmemiş) DTO olarak getirir
        // Kitap adı ve iade durumu gibi bilgileri içerir.
        [HttpGet("tumOduncIslemleri")]
        public async Task<ActionResult<IEnumerable<OduncIslemDto>>> GetTumOduncIslemleri()
        {
            var oduncIslemleri = await _context.OduncKayitlari // DbContext üzerinden doğrudan sorgulama
                                                .Include(o => o.Kitap) // Kitap bilgilerini de yükle
                                                .Include(o => o.Kullanici) // Kullanıcı bilgilerini de yükle
                                                .Select(o => new OduncIslemDto // DTO'ya dönüştür
                                                {
                                                    OduncIslemId = o.OduncId, // OduncId'yi OduncIslemId olarak kullanıyoruz
                                                    KullaniciId = o.KullaniciId,
                                                    KullaniciEmail = o.Kullanici.Email, // Yeni eklenen alan
                                                    KitapId = o.KitapId,
                                                    KitapAdi = o.Kitap.KitapAdi, // Kitap entity'sindeki 'Ad' alanı
                                                    OduncAlmaTarihi = o.OduncAlmaTarihi,
                                                    SonTeslimTarihi = o.SonTeslimTarihi,
                                                    TeslimEdildi = (o.Durum == "IadeEdildi"), // Durum alanından boolean çeviri
                                                    TeslimTarihi = o.IadeTarihi
                                                })
                                                .ToListAsync();
            return Ok(oduncIslemleri);
        }


        // POST: api/OduncIslemleri/oduncAl (Kitap Ödünç Alma - ID ile)
        [HttpPost("oduncAl")]
        public async Task<ActionResult<OduncKayit>> OduncAl([FromBody] OduncAlmaInputDto oduncAlmaDto)
        {
            var kullanici = await _kullaniciRepository.GetByIdAsync(oduncAlmaDto.KullaniciId);
            if (kullanici == null)
            {
                return BadRequest($"Kullanıcı ID {oduncAlmaDto.KullaniciId} bulunamadı.");
            }

            var kitap = await _kitapRepository.GetByIdAsync(oduncAlmaDto.KitapId);
            if (kitap == null)
            {
                return BadRequest($"Kitap ID {oduncAlmaDto.KitapId} bulunamadı.");
            }

            if (kitap.MevcutAdet <= 0)
            {
                return BadRequest($"Bu kitap şu anda ödünç alınamaz, stokta yok.");
            }

            // Kitabın mevcut adedini azalt
            kitap.MevcutAdet--;
            await _kitapRepository.UpdateAsync(kitap); // Kitabı da güncelliyoruz

            var oduncKayit = new OduncKayit
            {
                KullaniciId = oduncAlmaDto.KullaniciId,
                KitapId = oduncAlmaDto.KitapId,
                OduncAlmaTarihi = DateTime.Now,
                SonTeslimTarihi = DateTime.Now.AddDays(14),
                Durum = "OduncAlindi", // İlk durum
                IadeTarihi = null   // Henüz iade edilmediği için null
            };

            await _oduncKayitRepository.AddAsync(oduncKayit);
            await _oduncKayitRepository.SaveChangesAsync(); // Hem ödünç kaydını hem de kitap güncellemesini kaydet

            return CreatedAtAction(nameof(GetOduncKayit), new { id = oduncKayit.OduncId }, oduncKayit);
        }

       
        [HttpPost("oduncAlByEmail")]
        public async Task<IActionResult> OduncAlByEmail
        ([FromBody] OduncAlmaByEmailInputDto inputDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Kullanıcıyı e-posta ile bul
            var kullanici = await _kullaniciRepository.GetByEmailAsync(inputDto.KullaniciEmail);
            if (kullanici == null)
            {
                return NotFound("Belirtilen e-posta adresine sahip kullanıcı bulunamadı.");
            }

            // 2. Kitabı ID'si ile bul
            var kitap = await _kitapRepository.GetByIdAsync(inputDto.KitapId);
            if (kitap == null)
            {
                return NotFound("Kitap bulunamadı.");
            }

            // 3. Kitap stoğunu kontrol et
            if (kitap.MevcutAdet <= 0)
            {
                return BadRequest("Bu kitap şu anda mevcut değil veya stokta kalmadı.");
            }

            // 4. Kullanıcının daha önce bu kitabı iade etmemiş bir ödünç kaydı var mı kontrol et (opsiyonel ama iyi bir kontrol)
            var existingBorrow = await _context.OduncKayitlari
                                                 .FirstOrDefaultAsync(ok => ok.KullaniciId == kullanici.KullaniciId &&
                                                                             ok.KitapId == inputDto.KitapId &&
                                                                             ok.Durum != "IadeEdildi"); // veya !ok.TeslimEdildi
            if (existingBorrow != null)
            {
                return BadRequest("Bu kullanıcı bu kitabı zaten ödünç almış ve henüz iade etmemiş.");
            }


            // 5. Yeni ödünç kaydını oluştur
            var oduncKayit = new OduncKayit
            {
                KullaniciId = kullanici.KullaniciId, // Bulunan kullanıcının ID'sini kullan
                KitapId = inputDto.KitapId,
                OduncAlmaTarihi = DateTime.Now, // DateTime.UtcNow yerine DateTime.Now kullanıyorum
                SonTeslimTarihi = DateTime.Now.AddDays(14), // Varsayılan 14 gün
                Durum = "OduncAlindi",
                IadeTarihi = null
            };

            await _oduncKayitRepository.AddAsync(oduncKayit);

            // 6. Kitap stok adedini azalt
            kitap.MevcutAdet--;
            await _kitapRepository.UpdateAsync(kitap); // Kitap güncellemesini kaydet

            // 7. Değişiklikleri veritabanına kaydet
            await _oduncKayitRepository.SaveChangesAsync(); // Tek SaveChangesAsync ile her ikisini de kaydeder

            // Başarılı yanıt
            return CreatedAtAction(nameof(GetOduncKayit), new { id = oduncKayit.OduncId }, oduncKayit);
        }


      
        [HttpPost("iadeEt")] 
        public async Task<IActionResult> 
        IadeEt([FromBody] IadeInputDto iadeDto)
        {
            if (iadeDto == null || iadeDto.OduncIslemId <= 0) // ID'nin geçerliliğini kontrol et
            {
                return BadRequest("Geçersiz iade isteği. Ödünç İşlem ID'si gereklidir.");
            }

            var oduncKayit = await _context.OduncKayitlari // Context üzerinden include ile Kitap'ı da yükle
                                            .Include(o => o.Kitap)
                                            .FirstOrDefaultAsync(o => o.OduncId == iadeDto.OduncIslemId);

            if (oduncKayit == null)
            {
                return NotFound("Ödünç kaydı bulunamadı.");
            }

            if (oduncKayit.Durum == "IadeEdildi")
            {
                return BadRequest("Bu kayıt zaten iade edilmiş.");
            }

            // İş Kuralları: İade tarihini ve durumu güncelle
            oduncKayit.IadeTarihi = DateTime.Now;
            oduncKayit.Durum = "IadeEdildi";

            // Kitabın mevcut adedini artır
            if (oduncKayit.Kitap != null)
            {
                oduncKayit.Kitap.MevcutAdet++;
            }
            else
            {
                Console.WriteLine($"Uyarı: OduncId {iadeDto.OduncIslemId} için kitap bilgisi bulunamadı.");
            }

            await _context.SaveChangesAsync();

            return Ok("Kitap başarıyla iade edildi.");
        }

        // DELETE: api/OduncIslemleri/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOduncKayit(int id)
        {
            var oduncKayit = await _oduncKayitRepository.GetByIdAsync(id);
            if (oduncKayit == null)
            {
                return NotFound();
            }

            await _oduncKayitRepository.DeleteAsync(id);
            await _oduncKayitRepository.SaveChangesAsync();

            return NoContent();
        }
    } // OduncIslemleriController sınıfının sonu

    // API'nin DTO'ları burada tanımlanır
    namespace KutuphaneOtomasyonu.API.Models
    {
        // Kitap Ödünç Alma için kullanılan DTO (Kullanıcı ID ile)
        public class OduncAlmaInputDto
        {
            [Required(ErrorMessage = "Kullanıcı ID gereklidir.")]
            public int KullaniciId { get; set; }

            [Required(ErrorMessage = "Kitap ID gereklidir.")]
            [StringLength(255)] // KitapId'nin uzunluğunu Kitap.cs'deki tanımla eşleştirin
            public string KitapId { get; set; }
        }

        // Kitap Ödünç Alma için kullanılan DTO (Kullanıcı E-posta ile) - YENİ DTO BURADA
        public class OduncAlmaByEmailInputDto
        {
            [Required(ErrorMessage = "Kullanıcı e-posta adresi gereklidir.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
            public string KullaniciEmail { get; set; }

            [Required(ErrorMessage = "Kitap ID gereklidir.")]
            [StringLength(255)] // Kitap ID'nizin uzunluğuna göre ayarlayın
            public string KitapId { get; set; }
        }

        // Kitap İade Etme için kullanılan DTO
        public class IadeInputDto
        {
            [Required(ErrorMessage = "Ödünç İşlem ID gereklidir.")]
            public int OduncIslemId { get; set; } // Ödünç Kaydının ID'si (int)
        }

   
        public class OduncIslemDto
        {
            public int OduncIslemId { get; set; }
            public int KullaniciId { get; set; }
            public string KullaniciEmail { get; set; }
            public string KitapId { get; set; }
            public string KitapAdi { get; set; } 
            public DateTime OduncAlmaTarihi { get; set; }
            public DateTime SonTeslimTarihi { get; set; }
            public bool TeslimEdildi { get; set; } 
            public DateTime? TeslimTarihi { get; set; } 

        }
    }
}