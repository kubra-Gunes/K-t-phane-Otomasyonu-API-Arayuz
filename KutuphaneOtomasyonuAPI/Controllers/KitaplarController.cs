using System.ComponentModel.DataAnnotations;
using KutuphaneOtomasyonu.API.Controllers;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using KutuphaneOtomasyonu.API.Entities;

using KutuphaneOtomasyonu.API.Interfaces;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;

using System;

using System.ComponentModel.DataAnnotations; // DTO'lar için

using Microsoft.EntityFrameworkCore; // Attach, Entry metotları için



namespace KutuphaneOtomasyonu.API.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class KitaplarController : ControllerBase

    {

        private readonly IKitapRepository _kitapRepository;

        private readonly IYazarRepository _yazarRepository;

        private readonly IKategoriRepository _kategoriRepository;



        public KitaplarController(IKitapRepository kitapRepository, IYazarRepository yazarRepository, IKategoriRepository kategoriRepository)

        {

            _kitapRepository = kitapRepository;

            _yazarRepository = yazarRepository;

            _kategoriRepository = kategoriRepository;

        }



        // GET: api/Kitaplar

        // GET: api/Kitaplar

        [HttpGet]

        public async Task<ActionResult<IEnumerable<KitapDto>>> GetKitaplar([FromQuery] string searchText = null)

        {



            var kitaplarQuery = _kitapRepository.GetAll()

              .Include(k => k.Yazar)

              .Include(k => k.Kategori)

              .Include(k => k.KitapYorumlari); // Yorumları da dahil et

            IQueryable<Kitap> filtreliKitaplarQuery = kitaplarQuery;


            // Arama metni varsa filtreleme yap

            if (!string.IsNullOrWhiteSpace(searchText))

            {
                filtreliKitaplarQuery = kitaplarQuery.Where(k =>
       k.KitapAdi.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
       k.Yazar.Ad.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
       k.Yazar.Soyad.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
       k.Kategori.KategoriAdi.Contains(searchText, StringComparison.OrdinalIgnoreCase));


            }



            var kitaplar = await filtreliKitaplarQuery.ToListAsync();



            var kitapDtoList = kitaplar.Select(k => new KitapDto

            {

                KitapId = k.KitapId,

                KitapAdi = k.KitapAdi,

                YazarId = k.YazarId,

                YazarAdSoyad = $"{k.Yazar.Ad} {k.Yazar.Soyad}",

                KategoriId = k.KategoriId,

                KategoriAdi = k.Kategori.KategoriAdi,

                StokAdedi = k.StokAdedi,

                MevcutAdet = k.MevcutAdet,

                YayinYili = k.YayinYili,

                Aciklama = k.Aciklama,

                SayfaSayisi = k.SayfaSayisi,





                // Yorum verilerini hesapla

                YorumSayisi = k.KitapYorumlari?.Count() ?? 0,

                OrtalamaPuan = k.KitapYorumlari != null && k.KitapYorumlari.Any()

                     ? Math.Round(k.KitapYorumlari.Average(y => y.Puan), 2)

                     : 0 // Yorum yoksa 0 döndür

            }).ToList();



            return Ok(kitapDtoList);

        }



        // GET: api/Kitaplar/ISBN12345

        [HttpGet("{id}")]

        public async Task<ActionResult<KitapDto>> GetKitap(string id)

        {

            var kitap = await _kitapRepository.GetByIdAsync(id);



            if (kitap == null)

            {

                return NotFound();

            }



            var kitapDto = new KitapDto

            {

                KitapId = kitap.KitapId,

                KitapAdi = kitap.KitapAdi,

                StokAdedi = kitap.StokAdedi,

                MevcutAdet = kitap.MevcutAdet,

                YayinYili = kitap.YayinYili,

                Aciklama = kitap.Aciklama,

                SayfaSayisi = kitap.SayfaSayisi,

                YazarId = kitap.YazarId,

                YazarAdSoyad = kitap.Yazar != null ? $"{kitap.Yazar.Ad} {kitap.Yazar.Soyad}" : "Bilinmiyor",

                KategoriId = kitap.KategoriId,

                KategoriAdi = kitap.Kategori != null ? kitap.Kategori.KategoriAdi : "Bilinmiyor"

            };



            return Ok(kitapDto);

        }



        // GET: api/Kitaplar/byAuthor/1

        [HttpGet("byAuthor/{authorId}")]

        public async Task<ActionResult<IEnumerable<KitapDto>>> GetBooksByAuthor(int authorId)

        {

            var kitaplar = await _kitapRepository.GetBooksByAuthorIdAsync(authorId);

            if (kitaplar == null || !kitaplar.Any())

            {

                return NotFound($"Yazar ID'si {authorId} olan kitap bulunamadı.");

            }



            var kitapDtoList = kitaplar.Select(k => new KitapDto

            {

                KitapId = k.KitapId,

                KitapAdi = k.KitapAdi,

                StokAdedi = k.StokAdedi,

                MevcutAdet = k.MevcutAdet,

                YayinYili = k.YayinYili,

                Aciklama = k.Aciklama,

                SayfaSayisi = k.SayfaSayisi,

                YazarId = k.YazarId,

                YazarAdSoyad = k.Yazar != null ? $"{k.Yazar.Ad} {k.Yazar.Soyad}" : "Bilinmiyor",

                KategoriId = k.KategoriId,

                KategoriAdi = k.Kategori != null ? k.Kategori.KategoriAdi : "Bilinmiyor"

            }).ToList();



            return Ok(kitapDtoList);

        }



        // GET: api/Kitaplar/byCategory/Fantastik

        [HttpGet("byCategory/{categoryName}")]

        public async Task<ActionResult<IEnumerable<KitapDto>>> GetBooksByCategory(string categoryName)

        {

            var kitaplar = await _kitapRepository.GetBooksByCategoryNameAsync(categoryName);

            if (kitaplar == null || !kitaplar.Any())

            {

                return NotFound($"'{categoryName}' kategorisinde kitap bulunamadı.");

            }



            var kitapDtoList = kitaplar.Select(k => new KitapDto

            {

                KitapId = k.KitapId,

                KitapAdi = k.KitapAdi,

                StokAdedi = k.StokAdedi,

                MevcutAdet = k.MevcutAdet,

                YayinYili = k.YayinYili,

                Aciklama = k.Aciklama,

                SayfaSayisi = k.SayfaSayisi,

                YazarId = k.YazarId,

                YazarAdSoyad = k.Yazar != null ? $"{k.Yazar.Ad} {k.Yazar.Soyad}" : "Bilinmiyor",

                KategoriId = k.KategoriId,

                KategoriAdi = k.Kategori != null ? k.Kategori.KategoriAdi : "Bilinmiyor"

            }).ToList();



            return Ok(kitapDtoList);

        }



        // POST: api/Kitaplar

        [HttpPost]

        public async Task<ActionResult<KitapDto>> PostKitap([FromBody] CreateKitapDto createDto)

        {

            if (!await _yazarRepository.ExistsAsync(createDto.YazarId))

            {

                return BadRequest($"Yazar ID {createDto.YazarId} bulunamadı.");

            }

            if (!await _kategoriRepository.ExistsAsync(createDto.KategoriId))

            {

                return BadRequest($"Kategori ID {createDto.KategoriId} bulunamadı.");

            }



            string newKitapId = Guid.NewGuid().ToString();



            var yeniKitap = new Kitap

            {

                KitapId = newKitapId,

                KitapAdi = createDto.KitapAdi,

                YazarId = createDto.YazarId,

                KategoriId = createDto.KategoriId,

                StokAdedi = createDto.StokAdedi,

                MevcutAdet = createDto.MevcutAdet == 0 ? createDto.StokAdedi : createDto.MevcutAdet,

                YayinYili = createDto.YayinYili,

                Aciklama = createDto.Aciklama,

                SayfaSayisi = createDto.SayfaSayisi,

            };



            await _kitapRepository.AddAsync(yeniKitap);

            await _kitapRepository.SaveChangesAsync();



            var createdKitap = await _kitapRepository.GetByIdAsync(yeniKitap.KitapId);



            var createdKitapDto = new KitapDto

            {

                KitapId = createdKitap.KitapId,

                KitapAdi = createdKitap.KitapAdi,

                StokAdedi = createdKitap.StokAdedi,

                MevcutAdet = createdKitap.MevcutAdet,

                YayinYili = createdKitap.YayinYili,

                Aciklama = createdKitap.Aciklama,

                SayfaSayisi = createdKitap.SayfaSayisi,

                YazarId = createdKitap.YazarId,

                YazarAdSoyad = createdKitap.Yazar != null ? $"{createdKitap.Yazar.Ad} {createdKitap.Yazar.Soyad}" : "Bilinmiyor",

                KategoriId = createdKitap.KategoriId,

                KategoriAdi = createdKitap.Kategori != null ? createdKitap.Kategori.KategoriAdi : "Bilinmiyor"

            };



            return CreatedAtAction(nameof(GetKitap), new { id = createdKitapDto.KitapId }, createdKitapDto);

        }



        // PUT: api/Kitaplar/ISBN12345 (Güncellenmiş metot, UpdateKitapDto kullanılıyor)

        [HttpPut("{id}")]

        public async Task<IActionResult> PutKitap(string id, [FromBody] UpdateKitapDto updateDto)

        {

            if (id != updateDto.KitapId)

            {

                return BadRequest("URL'deki Kitap ID'si ile istek gövdesindeki Kitap ID'si uyuşmuyor.");

            }



            // Mevcut kitabı veritabanından getir

            var kitapToUpdate = await _kitapRepository.GetByIdAsync(id);

            if (kitapToUpdate == null)

            {

                return NotFound($"Kitap ID {id} bulunamadı.");

            }



            // Yazar ve kategorinin varlığını kontrol et

            if (!await _yazarRepository.ExistsAsync(updateDto.YazarId))

            {

                return BadRequest($"Yazar ID {updateDto.YazarId} bulunamadı.");

            }

            if (!await _kategoriRepository.ExistsAsync(updateDto.KategoriId))

            {

                return BadRequest($"Kategori ID {updateDto.KategoriId} bulunamadı.");

            }



            // DTO'daki verileri mevcut entity'ye aktar

            kitapToUpdate.KitapAdi = updateDto.KitapAdi;

            kitapToUpdate.YazarId = updateDto.YazarId;

            kitapToUpdate.KategoriId = updateDto.KategoriId;

            kitapToUpdate.StokAdedi = updateDto.StokAdedi;

            kitapToUpdate.MevcutAdet = updateDto.MevcutAdet;

            kitapToUpdate.YayinYili = updateDto.YayinYili;

            kitapToUpdate.Aciklama = updateDto.Aciklama;

            kitapToUpdate.SayfaSayisi = updateDto.SayfaSayisi; // SayfaSayisi güncellendi



            // UpdateAsync metodunuz entity'nin durumunu Modified olarak ayarlamalıdır.

            await _kitapRepository.UpdateAsync(kitapToUpdate);

            await _kitapRepository.SaveChangesAsync();



            return NoContent(); // 204 No Content

        }



        // DELETE: api/Kitaplar/ISBN12345

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteKitap(string id)

        {

            var kitap = await _kitapRepository.GetByIdAsync(id);

            if (kitap == null)

            {

                return NotFound();

            }



            await _kitapRepository.DeleteAsync(id);

            await _kitapRepository.SaveChangesAsync();



            return NoContent();

        }

    }





    public class CreateKitapDto

    {

        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]

        [StringLength(255, ErrorMessage = "Kitap adı en fazla 255 karakter olabilir.")]

        public string KitapAdi { get; set; }



        [Required(ErrorMessage = "Yazar ID boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Yazar ID giriniz.")]

        public int YazarId { get; set; }



        [Required(ErrorMessage = "Kategori ID boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Kategori ID giriniz.")]

        public int KategoriId { get; set; }



        [Required(ErrorMessage = "Stok adedi boş bırakılamaz.")]

        [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]

        public int StokAdedi { get; set; }



        [Range(0, int.MaxValue, ErrorMessage = "Mevcut adet negatif olamaz.")]

        public int MevcutAdet { get; set; }



        [Required(ErrorMessage = "Yayın yılı boş bırakılamaz.")]

        [Range(1000, 9999, ErrorMessage = "Geçerli bir yayın yılı giriniz.")]

        public int YayinYili { get; set; }



        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]

        public string Aciklama { get; set; }



        [Required(ErrorMessage = "Sayfa sayısı boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir sayfa sayısı giriniz.")]

        public int SayfaSayisi { get; set; }

    }



    public class KitapDto

    {

        public string KitapId { get; set; }

        public string KitapAdi { get; set; }

        public int YazarId { get; set; }

        public string YazarAdSoyad { get; set; }

        public int KategoriId { get; set; }

        public string KategoriAdi { get; set; }

        public int StokAdedi { get; set; }

        public int MevcutAdet { get; set; }

        public int YayinYili { get; set; }

        public string Aciklama { get; set; }

        public int SayfaSayisi { get; set; }

        public double OrtalamaPuan { get; set; } // Yeni eklenecek

        public int YorumSayisi { get; set; }

    }





    public class UpdateKitapDto

    {

        [Required(ErrorMessage = "Kitap ID boş bırakılamaz.")]

        public string KitapId { get; set; } // Update için KitapId de olmalı



        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]

        [StringLength(255, ErrorMessage = "Kitap adı en fazla 255 karakter olabilir.")]

        public string KitapAdi { get; set; }



        [Required(ErrorMessage = "Yazar ID boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Yazar ID giriniz.")]

        public int YazarId { get; set; }



        [Required(ErrorMessage = "Kategori ID boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Kategori ID giriniz.")]

        public int KategoriId { get; set; }



        [Required(ErrorMessage = "Stok adedi boş bırakılamaz.")]

        [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]

        public int StokAdedi { get; set; }



        [Range(0, int.MaxValue, ErrorMessage = "Mevcut adet negatif olamaz.")]

        public int MevcutAdet { get; set; }



        [Required(ErrorMessage = "Yayın yılı boş bırakılamaz.")]

        [Range(1000, 9999, ErrorMessage = "Geçerli bir yayın yılı giriniz.")]

        public int YayinYili { get; set; }



        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]

        public string Aciklama { get; set; }



        [Required(ErrorMessage = "Sayfa sayısı boş bırakılamaz.")]

        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir sayfa sayısı giriniz.")]

        public int SayfaSayisi { get; set; }

    }

}