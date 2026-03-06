using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneOtomasyonu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategorilerController : ControllerBase
    {
        private readonly IKategoriRepository _kategoriRepository;

        public KategorilerController(IKategoriRepository kategoriRepository)
        {
            _kategoriRepository = kategoriRepository;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kategori>>> GetKategoriler([FromQuery] string searchText = null)
        {
            var kategoriler = await _kategoriRepository.GetAllAsync();


            if (!string.IsNullOrWhiteSpace(searchText))
            {
                kategoriler = kategoriler.Where(k =>
                    k.KategoriAdi.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Ok(kategoriler);
        }

 
        [HttpGet("{id}")]
        public async Task<ActionResult<Kategori>> GetKategori(int id)
        {
            var kategori = await _kategoriRepository.GetByIdAsync(id);

            if (kategori == null)
            {
                return NotFound();
            }

            return Ok(kategori);
        }

   
        [HttpPost]
        public async Task<ActionResult<Kategori>> PostKategori([FromBody] CreateKategoriDto createDto) 
        {
            var existingKategori = await _kategoriRepository.GetCategoryByNameAsync(createDto.KategoriAdi);
            if (existingKategori != null)
            {
                return Conflict("Bu kategori adı zaten kullanımda.");
            }

            var yeniKategori = new Kategori
            {
                KategoriAdi = createDto.KategoriAdi,
                Kitaplari = new List<Kitap>() 
            };

            await _kategoriRepository.AddAsync(yeniKategori);
            await _kategoriRepository.SaveChangesAsync();

            // Oluşturulan kategori başarılı yanıtla döndürülür.
            return CreatedAtAction(nameof(GetKategori), new { id = yeniKategori.KategoriId }, yeniKategori);
        }


        // PUT: api/Kategoriler/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKategori(int id, [FromBody] UpdateKategoriDto model) // DTO kullanıyoruz
        {
            if (id != model.KategoriId)
            {
                return BadRequest("URL'deki ID ile gönderilen kategori ID'si uyuşmuyor.");
            }

            var existingKategori = await _kategoriRepository.GetByIdAsync(id);

            if (existingKategori == null)
            {
                return NotFound("Güncellenecek kategori bulunamadı.");
            }
            if (existingKategori.KategoriAdi != model.KategoriAdi) 
            {
                var kategoriWithSameName = await _kategoriRepository.GetCategoryByNameAsync(model.KategoriAdi);
                if (kategoriWithSameName != null)
                {
                    return Conflict("Bu kategori adı zaten başka bir kategori tarafından kullanılıyor.");
                }
            }

            existingKategori.KategoriAdi = model.KategoriAdi;
            await _kategoriRepository.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        // DELETE: api/Kategoriler/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKategori(int id)
        {
            var kategori = await _kategoriRepository.GetByIdAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }

            await _kategoriRepository.DeleteAsync(id);
            await _kategoriRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}

public class CreateKategoriDto
{
    [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
    [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
    public string KategoriAdi { get; set; }
  
}

public class UpdateKategoriDto
{
    [Required] // Güncelleme işlemi için ID'nin gelmesi zorunludur
    public int KategoriId { get; set; }

    [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
    [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
    public string KategoriAdi { get; set; }
}