// KutuphaneOtomasyonu.WPF.Models/KitapYorumuDto.cs
using System;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class BookCommentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("kitapId")]
        public string KitapId { get; set; }

        [JsonPropertyName("kitapAd")]
        public string KitapAd { get; set; }

        [JsonPropertyName("kullaniciId")]
        public int KullaniciId { get; set; }

        [JsonPropertyName("kullaniciAdSoyad")]
        public string KullaniciAdSoyad { get; set; }

        [JsonPropertyName("puan")]
        public int Puan { get; set; }

        [JsonPropertyName("yorumMetni")]
        public string YorumMetni { get; set; }

        [JsonPropertyName("yorumTarihi")]
        public DateTime YorumTarihi { get; set; }
    }
}