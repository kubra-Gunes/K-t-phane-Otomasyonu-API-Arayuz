using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization; // JsonPropertyName için ekleyin

// Models/CategoryDto.cs
namespace KutuphaneOtomasyonu.WPF.Models
{
    public class CategoryDto
    {
        [JsonPropertyName("kategoriId")]
        public int KategoriId { get; set; }

        [JsonPropertyName("kategoriAdi")]
        public string KategoriAdi { get; set; }
    }
}