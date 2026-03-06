// KutuphaneOtomasyonu.WPF.Models/CreateKitapYorumuDto.cs
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class CreateBookCommentDto
    {
        [JsonPropertyName("kitapId")]
        public string KitapId { get; set; }

        [JsonPropertyName("puan")]
        public int Puan { get; set; }

        [JsonPropertyName("yorumMetni")]
        public string YorumMetni { get; set; }
    }
}