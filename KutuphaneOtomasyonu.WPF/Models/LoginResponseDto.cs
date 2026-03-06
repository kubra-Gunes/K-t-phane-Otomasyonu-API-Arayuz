using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization; 

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class LoginResponseDto
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; } 

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("kullaniciId")]
        public int KullaniciId { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; }
    }
}