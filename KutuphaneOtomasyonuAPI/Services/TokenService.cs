using KutuphaneOtomasyonu.API.Entities;
using KutuphaneOtomasyonu.API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Diagnostics; // Debug sınıfı için

namespace KutuphaneOtomasyonu.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            _issuer = config["Jwt:Issuer"];
            _audience = config["Jwt:Audience"];
        }

        public string CreateToken(Kullanici kullanici)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, kullanici.KullaniciId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var generatedToken = tokenHandler.WriteToken(token);

            // Hata ayıklama için token'ı ve kullanılan anahtarı yazdır.
            Debug.WriteLine("-------------------------------------------------------");
            Debug.WriteLine($"TokenService: Yeni token olusturuldu.");
            Debug.WriteLine($"TokenService: Anahtar uzunlugu: {_key.Key.Length} byte");
            Debug.WriteLine($"TokenService: Olusturulan Token: {generatedToken}");
            Debug.WriteLine("-------------------------------------------------------");

            return generatedToken;
        }
    }
}
