// KutuphaneOtomasyonu.WPF.Models/UpdateBookDto.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class UpdateBookDto
    {
        [JsonPropertyName("kitapId")]
        [Required(ErrorMessage = "Kitap ID boş bırakılamaz.")]
        public string KitapId { get; set; } // Güncelleme için KitapId gerekli

        [JsonPropertyName("kitapAdi")]
        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]
        [StringLength(255, ErrorMessage = "Kitap adı en fazla 255 karakter olabilir.")]
        public string KitapAdi { get; set; }

        [JsonPropertyName("yazarId")]
        [Required(ErrorMessage = "Yazar ID boş bırakılamaz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Yazar ID giriniz.")]
        public int YazarId { get; set; }

        [JsonPropertyName("kategoriId")]
        [Required(ErrorMessage = "Kategori ID boş bırakılamaz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir Kategori ID giriniz.")]
        public int KategoriId { get; set; }

        [JsonPropertyName("stokAdedi")]
        [Required(ErrorMessage = "Stok adedi boş bırakılamaz.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]
        public int StokAdedi { get; set; }

        [JsonPropertyName("mevcutAdet")]
        [Range(0, int.MaxValue, ErrorMessage = "Mevcut adet negatif olamaz.")]
        public int MevcutAdet { get; set; }

        [JsonPropertyName("yayinYili")]
        [Required(ErrorMessage = "Yayın yılı boş bırakılamaz.")]
        [Range(1000, 9999, ErrorMessage = "Geçerli bir yayın yılı giriniz.")]
        public int YayinYili { get; set; }

        [JsonPropertyName("aciklama")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string Aciklama { get; set; }

        [JsonPropertyName("sayfaSayisi")]
        [Required(ErrorMessage = "Sayfa sayısı boş bırakılamaz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir sayfa sayısı giriniz.")]
        public int SayfaSayisi { get; set; }
    }
}