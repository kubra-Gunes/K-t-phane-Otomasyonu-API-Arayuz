using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class UpdateAuthorDto
    {
        [JsonPropertyName("yazarId")]
        [Required(ErrorMessage = "Yazar ID boş bırakılamaz.")]
        public int YazarId { get; set; }

        [JsonPropertyName("ad")]
        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Ad { get; set; }

        [JsonPropertyName("soyad")]
        [Required(ErrorMessage = "Yazar soyadı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string Soyad { get; set; }

        [JsonPropertyName("biyografi")]
        [StringLength(1000, ErrorMessage = "Biyografi en fazla 1000 karakter olabilir.")]
        public string Biyografi { get; set; }
    }
}