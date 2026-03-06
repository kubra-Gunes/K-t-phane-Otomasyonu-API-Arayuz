// KutuphaneOtomasyonu.API.Controllers/YazarlarController.cs
using Microsoft.AspNetCore.Mvc;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // DTO'lar için
using System.Linq; // Any() için

namespace KutuphaneOtomasyonu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YazarlarController : ControllerBase
    {
        private readonly IYazarRepository _yazarRepository;

        public YazarlarController(IYazarRepository yazarRepository)
        {
            _yazarRepository = yazarRepository;
        }

        // GET: api/Yazarlar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Yazar>>> GetYazarlar([FromQuery] string searchText = null)
        {
            var yazarlar = await _yazarRepository.GetAllAsync();

            // Arama metni varsa filtreleme yap
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                yazarlar = yazarlar.Where(y =>
                    y.Ad.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    y.Soyad.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (y.Biyografi != null && y.Biyografi.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            return Ok(yazarlar);
        }

        // GET: api/Yazarlar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Yazar>> GetYazar(int id)
        {
            var yazar = await _yazarRepository.GetByIdAsync(id);

            if (yazar == null)
            {
                return NotFound();
            }

            return Ok(yazar);
        }

        // GET: api/Yazarlar/byName?firstName=Ahmet&lastName=Ümit
        [HttpGet("byName")]
        public async Task<ActionResult<IEnumerable<Yazar>>> GetAuthorsByName(string firstName, string lastName)
        {
            var yazarlar = await _yazarRepository.GetAuthorsByNameAsync(firstName, lastName);
            if (yazarlar == null || !yazarlar.Any())
            {
                return NotFound($"Adı '{firstName}' ve Soyadı '{lastName}' olan yazar bulunamadı.");
            }
            return Ok(yazarlar);
        }

        // POST: api/Yazarlar
        [HttpPost]
        public async Task<ActionResult<Yazar>> PostYazar([FromBody] CreateYazarDto createDto)
        {
            var yeniYazar = new Yazar
            {
                Ad = createDto.Ad,
                Soyad = createDto.Soyad,
                Biyografi = createDto.Biyografi
            };

            await _yazarRepository.AddAsync(yeniYazar);
            await _yazarRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetYazar), new { id = yeniYazar.YazarId }, yeniYazar);
        }

        // PUT: api/Yazarlar/5 (Güncellenmiş metot, UpdateYazarDto kullanılıyor)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutYazar(int id, [FromBody] UpdateYazarDto updateDto)
        {
            if (id != updateDto.YazarId)
            {
                return BadRequest("URL'deki Yazar ID'si ile istek gövdesindeki Yazar ID'si uyuşmuyor.");
            }

            // Mevcut yazarı veritabanından getir
            var yazarToUpdate = await _yazarRepository.GetByIdAsync(id);
            if (yazarToUpdate == null)
            {
                return NotFound($"Yazar ID {id} bulunamadı.");
            }

            // DTO'daki verileri mevcut entity'ye aktar
            yazarToUpdate.Ad = updateDto.Ad;
            yazarToUpdate.Soyad = updateDto.Soyad;
            yazarToUpdate.Biyografi = updateDto.Biyografi;

            // UpdateAsync metodunuz entity'nin durumunu Modified olarak ayarlamalıdır.
            await _yazarRepository.UpdateAsync(yazarToUpdate);
            await _yazarRepository.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/Yazarlar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteYazar(int id)
        {
            var yazar = await _yazarRepository.GetByIdAsync(id);
            if (yazar == null)
            {
                return NotFound(); // Yazar bulunamadıysa 404
            }

            await _yazarRepository.DeleteAsync(id); // Yazar silme işlemi
            await _yazarRepository.SaveChangesAsync(); // Değişiklikleri kaydet

            return NoContent(); // Başarılı silme için 204 No Content
        }
    }

    // --- DTO (Data Transfer Object) Tanımları ---

    /// <summary>
    /// Yazar oluşturma işlemi için kullanılan DTO.
    /// </summary>
    public class CreateYazarDto
    {
        [Required(ErrorMessage = "Yazarın adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Yazarın soyadı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string Soyad { get; set; }

        [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
        public string Biyografi { get; set; }
    }

    /// <summary>
    /// Yazar güncelleme işlemi için kullanılan DTO.
    /// Sadece güncellenebilir temel bilgileri içerir.
    /// </summary>
    public class UpdateYazarDto
    {
        [Required(ErrorMessage = "Yazar ID boş bırakılamaz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Yazar ID giriniz.")]
        public int YazarId { get; set; } // Güncelleme için ID gereklidir.

        [Required(ErrorMessage = "Yazarın adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Yazarın soyadı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string Soyad { get; set; }

        [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
        public string Biyografi { get; set; }
    }
}