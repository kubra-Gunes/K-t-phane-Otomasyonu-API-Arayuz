using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class CreateCategoryDto
    {
        [JsonPropertyName("kategoriAdi")]
        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string KategoriAdi { get; set; }
    }
}