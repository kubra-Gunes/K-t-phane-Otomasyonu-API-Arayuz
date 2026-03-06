using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class CreateAuthorDto
    {
        [JsonPropertyName("ad")]
        [Required(ErrorMessage = "Yazarın adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Ad { get; set; }

        [JsonPropertyName("soyad")]
        [Required(ErrorMessage = "Yazarın soyadı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string Soyad { get; set; }

        [JsonPropertyName("biyografi")]
        [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
        public string Biyografi { get; set; }
    }
}