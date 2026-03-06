using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

// KutuphaneOtomasyonu.WPF.Models/UserDto.cs (Örnek)
namespace KutuphaneOtomasyonu.WPF.Models
{
    public class UserDto
    {
        [JsonPropertyName("kullaniciId")]
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string FullName => $"{Ad} {Soyad}"; // UI için kolaylık
    }
}