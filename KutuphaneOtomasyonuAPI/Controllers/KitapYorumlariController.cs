// KutuphaneOtomasyonu.API.Controllers/KitapYorumlariController.cs
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using KutuphaneOtomasyonu.API.Data; // DbContext için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // ClaimTypes için
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bu controller içindeki tüm metotların yetkilendirme gerektirmesini sağlar
    public class KitapYorumlariController : ControllerBase
    {
        private readonly IRepository<KitapYorumu> _yorumRepository;
        private readonly IKitapRepository _kitapRepository;
        private readonly IKullaniciRepository _kullaniciRepository;

        public KitapYorumlariController(
            IRepository<KitapYorumu> yorumRepository,
            IKitapRepository kitapRepository,
            IKullaniciRepository kullaniciRepository)
        {
            _yorumRepository = yorumRepository;
            _kitapRepository = kitapRepository;
            _kullaniciRepository = kullaniciRepository;
        }

        [HttpGet("kitap/{kitapId}")]
        [AllowAnonymous] // Herkes bu metodu çağırabilir
        public async Task<ActionResult<IEnumerable<KitapYorumuDto>>> GetCommentsByBookId(string kitapId)
        {
            var tumYorumlar = await _yorumRepository.GetAllAsync();
            var yorumlar = tumYorumlar.Where(y => y.KitapId == kitapId);

            if (yorumlar == null || !yorumlar.Any())
            {
                return NotFound("Bu kitaba ait yorum bulunamadı.");
            }

            var kitapYorumlari = new List<KitapYorumuDto>();
            foreach (var yorum in yorumlar)
            {
                var kullanici = await _kullaniciRepository.GetByIdAsync(yorum.KullaniciId);
                var kitap = await _kitapRepository.GetByIdAsync(yorum.KitapId);

                kitapYorumlari.Add(new KitapYorumuDto
                {
                    Id = yorum.Id,
                    KitapId = yorum.KitapId,
                    KitapAd = kitap?.KitapAdi,
                    KullaniciId = yorum.KullaniciId,
                    KullaniciAdSoyad = $"{kullanici?.Ad} {kullanici?.Soyad}",
                    Puan = yorum.Puan,
                    YorumMetni = yorum.YorumMetni,
                    YorumTarihi = yorum.YorumTarihi
                });
            }

            return Ok(kitapYorumlari);
        }

        [HttpPost]
        public async Task<ActionResult<KitapYorumu>> PostKitapYorumu(CreateKitapYorumuDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var kullaniciId))
            {
                return Unauthorized("Kullanıcı kimliği token'dan alınamadı.");
            }
         
            // Kullanıcının bu kitaba zaten yorum yapıp yapmadığını kontrol et.
            var existingComment = (await _yorumRepository.GetAllAsync())
                                    .FirstOrDefault(y => y.KitapId == createDto.KitapId && y.KullaniciId == kullaniciId);

            if (existingComment != null)
            {
                // Eğer bir yorum bulunursa, hata mesajı dön.
                return BadRequest("Bu kitaba zaten yorum yaptınız. Birden fazla yorum eklenemez.");
            }

            var yeniYorum = new KitapYorumu
            {
                KitapId = createDto.KitapId,
                KullaniciId = kullaniciId,
                Puan = createDto.Puan,
                YorumMetni = createDto.YorumMetni,
                YorumTarihi = DateTime.Now
            };

            await _yorumRepository.AddAsync(yeniYorum);
            await _yorumRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentsByBookId), new { kitapId = yeniYorum.KitapId }, yeniYorum);
        }
        // GET: api/KitapYorumlari/kullanici - Giriş yapmış kullanıcının tüm yorumlarını getirir.
        [HttpGet("kullanici")]
        public async Task<ActionResult<IEnumerable<KitapYorumuDto>>> GetCommentsByUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var kullaniciId))
            {
                return Unauthorized("Kullanıcı kimliği token'dan alınamadı.");
            }

            var tumYorumlar = await _yorumRepository.GetAllAsync();
            var kullaniciYorumlari = tumYorumlar.Where(y => y.KullaniciId == kullaniciId).ToList();

            if (kullaniciYorumlari == null || !kullaniciYorumlari.Any())
            {
                return NotFound("Kullanıcıya ait yorum bulunamadı.");
            }

            var yorumDtoListesi = new List<KitapYorumuDto>();
            foreach (var yorum in kullaniciYorumlari)
            {
                var kitap = await _kitapRepository.GetByIdAsync(yorum.KitapId);

                yorumDtoListesi.Add(new KitapYorumuDto
                {
                    Id = yorum.Id,
                    KitapId = yorum.KitapId,
                    KitapAd = kitap?.KitapAdi,
                    KullaniciId = yorum.KullaniciId,
                    KullaniciAdSoyad = null,
                    Puan = yorum.Puan,
                    YorumMetni = yorum.YorumMetni,
                    YorumTarihi = yorum.YorumTarihi
                });
            }

            return Ok(yorumDtoListesi);
        }

        [HttpPut("{yorumId}")]
        public async Task<ActionResult> PutKitapYorumu(int yorumId, [FromBody] UpdateKitapYorumuDto updateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var kullaniciId))
            {
                return Unauthorized("Kullanıcı kimliği token'dan alınamadı.");
            }

            var mevcutYorum = await _yorumRepository.GetByIdAsync(yorumId);
            if (mevcutYorum == null)
            {
                return NotFound("Güncellenecek yorum bulunamadı.");
            }

            // Yorumun giriş yapmış kullanıcıya ait olup olmadığını kontrol et
            if (mevcutYorum.KullaniciId != kullaniciId)
            {
                return Forbid("Sadece kendi yorumlarınızı güncelleyebilirsiniz.");
            }

            // DTO'daki verilerle mevcut yorumu güncelle
            mevcutYorum.YorumMetni = updateDto.YorumMetni;
            mevcutYorum.Puan = updateDto.Puan;
            // YorumTarihi otomatik olarak güncellenmez, sadece yorum metni ve puan güncellenir.

            await _yorumRepository.UpdateAsync(mevcutYorum);
            await _yorumRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("yorumlar/kullanici/{kullaniciId}")]
        public async Task<ActionResult<IEnumerable<KitapYorumu>>> GetYorumlarByKullaniciId(int kullaniciId)
        {
            var yorumlar = await _yorumRepository.GetAllAsync();
            var kullaniciYorumlari = yorumlar
                .Where(y => y.KullaniciId == kullaniciId)
                .ToList();

            if (kullaniciYorumlari == null || !kullaniciYorumlari.Any())
            {
                return NotFound("Bu kullanıcıya ait yorum bulunamadı.");
            }

            return Ok(kullaniciYorumlari);
        }

        [HttpDelete("yorumlar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteYorum(int id)
        {
            var yorum = await _yorumRepository.GetByIdAsync(id); // ✅ _context değil, repository kullanıyoruz
            if (yorum == null)
            {
                return NotFound();
            }

            await _yorumRepository.DeleteAsync(yorum.Id);
            await _yorumRepository.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("{yorumId}")]
        public async Task<ActionResult> DeleteKitapYorumu(int yorumId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var kullaniciId))
            {
                return Unauthorized("Kullanıcı kimliği token'dan alınamadı.");
            }

            var silinecekYorum = await _yorumRepository.GetByIdAsync(yorumId);
            if (silinecekYorum == null)
            {
                return NotFound("Silinecek yorum bulunamadı.");
            }

            // Yorumun giriş yapmış kullanıcıya ait olup olmadığını kontrol et
            if (silinecekYorum.KullaniciId != kullaniciId)
            {
                return Forbid("Sadece kendi yorumlarınızı silebilirsiniz.");
            }

            // Hatanın olduğu satır burasıydı. Metoda yorum nesnesi yerine yorumun ID'sini gönderiyoruz.
            await _yorumRepository.DeleteAsync(silinecekYorum.Id);
            await _yorumRepository.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/KitapYorumlari/kitap/{kitapId}/istatistik
        [HttpGet("kitap/{kitapId}/istatistik")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetBookCommentStatistics(string kitapId)
        {
            var yorumlar = (await _yorumRepository.GetAllAsync())
                            .Where(y => y.KitapId == kitapId)
                            .ToList();

            if (!yorumlar.Any())
            {
                return NotFound("Bu kitaba ait yorum bulunamadı.");
            }

            var yorumSayisi = yorumlar.Count;
            var ortalamaPuan = yorumlar.Average(y => y.Puan);

            return Ok(new
            {
                KitapId = kitapId,
                YorumSayisi = yorumSayisi,
                OrtalamaPuan = Math.Round(ortalamaPuan, 2)
            });
        }

    }




    public class KitapYorumuDto
    {
        public int Id { get; set; }
        public string KitapId { get; set; }
        public string KitapAd { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdSoyad { get; set; }
        public int Puan { get; set; }
        public string YorumMetni { get; set; }
        public DateTime YorumTarihi { get; set; }
    }

    public class CreateKitapYorumuDto
    {
        [Required(ErrorMessage = "Kitap ID'si zorunludur.")]
        public string KitapId { get; set; }

        [Required(ErrorMessage = "Puan zorunludur.")]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int Puan { get; set; }

        [MaxLength(500, ErrorMessage = "Yorum metni 500 karakterden uzun olamaz.")]
        public string YorumMetni { get; set; }
    }

    public class UpdateKitapYorumuDto
    {
        [Required(ErrorMessage = "Puan zorunludur.")]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int Puan { get; set; }

        [MaxLength(500, ErrorMessage = "Yorum metni 500 karakterden uzun olamaz.")]
        public string YorumMetni { get; set; }
    }

}
