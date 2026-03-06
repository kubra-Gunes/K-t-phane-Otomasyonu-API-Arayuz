using Microsoft.AspNetCore.Mvc;
using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using KutuphaneOtomasyonu.API.Services; // ITokenService için
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; // [Authorize] için
using System.Security.Claims; // ClaimTypes için

namespace KutuphaneOtomasyonu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullanicilarController : ControllerBase
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly ITokenService _tokenService; // Eğer kullanılıyorsa

        public KullanicilarController(IKullaniciRepository kullaniciRepository, ITokenService tokenService)
        {
            _kullaniciRepository = kullaniciRepository;
            _tokenService = tokenService;
        }

    
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kullanici>>> GetKullanicilar()
        {
            var kullanicilar = await _kullaniciRepository.GetAllAsync();
            return Ok(kullanicilar);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Kullanici>> GetKullanici(int id)
        {


            var kullanici = await _kullaniciRepository.GetByIdAsync(id);

            if (kullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(kullanici);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<Kullanici>> RegisterKullanici([FromBody] RegisterUserDto registerDto)
        {
            if (await _kullaniciRepository.UserExistsByEmailAsync(registerDto.Email))
            {
                return Conflict("Bu e-posta adresi zaten kullanımda.");
            }

            // ŞİFREYİ HASH'LEME
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var newKullanici = new Kullanici
            {
                Ad = registerDto.Ad,
                Soyad = registerDto.Soyad,
                Email = registerDto.Email,
                SifreHash = hashedPassword, // HASH'LENMİŞ ŞİFREYİ KAYDET
                KayitTarihi = System.DateTime.Now,
                Rol = "Uye"
            };

            await _kullaniciRepository.AddAsync(newKullanici);
            await _kullaniciRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKullanici), new { id = newKullanici.KullaniciId }, newKullanici);
        }
   

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginKullanici([FromBody] LoginUserDto loginDto)
        {
            var kullanici = await _kullaniciRepository.GetUserByEmailAsync(loginDto.Email);
            if (kullanici == null || kullanici.SifreHash != loginDto.Password)
            {
                return Unauthorized("Geçersiz e-posta veya şifre.");
            }

            var token = _tokenService.CreateToken(kullanici);
            return Ok(new { Token = token, KullaniciId = kullanici.KullaniciId, Rol = kullanici.Rol });
        }

        // PUT: api/Kullanicilar/5 - Giriş yapmış kullanıcılar kendi bilgilerini güncelleyebilir, Adminler herkesinkini.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKullanici(int id, [FromBody] UpdateKullaniciDto model) // DTO kullanıyoruz
        {
            if (id != model.KullaniciId)
            {
                return BadRequest("URL'deki ID ile gönderilen kullanıcı ID'si uyuşmuyor.");
            }

            // 1. Mevcut kullanıcıyı veritabanından al. Bu nesne DbContext tarafından izlenecektir.
            var existingKullanici = await _kullaniciRepository.GetByIdAsync(id);

            if (existingKullanici == null)
            {
                return NotFound("Güncellenecek kullanıcı bulunamadı.");
            }

            // 2. Mevcut (izlenen) nesnenin özelliklerini gelen DTO'daki değerlerle güncelle.
            existingKullanici.Ad = model.Ad;
            existingKullanici.Soyad = model.Soyad;
            existingKullanici.Email = model.Email;
            existingKullanici.Rol = model.Rol;
 
            await _kullaniciRepository.SaveChangesAsync(); // Değişiklikleri kaydeder

            return NoContent(); // 204 No Content
        }

        // DELETE: api/Kullanicilar/5 - Sadece Admin rolündeki kullanıcılar silebilir.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKullanici(int id)
        {
            var kullanici = await _kullaniciRepository.GetByIdAsync(id);
            if (kullanici == null)
            {
                return NotFound("Silinecek kullanıcı bulunamadı.");
            }

            await _kullaniciRepository.DeleteAsync(id);
            await _kullaniciRepository.SaveChangesAsync();

            return NoContent();
        }
    }

    // --- DTO (Data Transfer Object) Tanımları ---
    // Bu DTO'ları genellikle projenizin "Models" veya "DTOs" klasöründe tutmanız önerilir.

    /// <summary>
    /// Kullanıcı kayıt işlemi için kullanılan DTO.
    /// </summary>
    public class RegisterUserDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // API'den gelen şifre
        // Ekstra alanlar eklenebilir (örn: TelefonNumarasi)
    }

    /// <summary>
    /// Kullanıcı giriş işlemi için kullanılan DTO.
    /// </summary>
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

// KutuphaneOtomasyonu.API.DTOs/UpdateKullaniciDto.cs (Örnek Dizin)
public class UpdateKullaniciDto
{
    public int KullaniciId { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Email { get; set; }
    public string Rol { get; set; }

}