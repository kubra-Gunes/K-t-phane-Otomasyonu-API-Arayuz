// KutuphaneOtomasyonu.WPF.Models/UpdateCategoryDto.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class UpdateCategoryDto
    {
        [JsonPropertyName("kategoriId")]
        [Required(ErrorMessage = "Kategori ID boş bırakılamaz.")]
        public int KategoriId { get; set; }

        [JsonPropertyName("kategoriAdi")]
        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string KategoriAdi { get; set; }
    }
}