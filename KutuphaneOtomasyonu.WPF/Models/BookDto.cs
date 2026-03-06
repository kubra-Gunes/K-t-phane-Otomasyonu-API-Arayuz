using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization; // JsonPropertyName için ekleyin

namespace KutuphaneOtomasyonu.WPF.Models
{
    public class BookDto
    {
        [JsonPropertyName("kitapId")]
        public string KitapId { get; set; }

        [JsonPropertyName("kitapAdi")]
        public string KitapAdi { get; set; }

        [JsonPropertyName("yazarId")]
        public int YazarId { get; set; }

        [JsonPropertyName("yazarAdSoyad")]
        public string YazarAdSoyad { get; set; }

        [JsonPropertyName("kategoriId")]
        public int KategoriId { get; set; }

        [JsonPropertyName("kategoriAdi")]
        public string KategoriAdi { get; set; }

        [JsonPropertyName("stokAdedi")]
        public int StokAdedi { get; set; }

        [JsonPropertyName("mevcutAdet")]
        public int MevcutAdet { get; set; }

        [JsonPropertyName("yayinYili")]
        public int YayinYili { get; set; }

        [JsonPropertyName("aciklama")]
        public string Aciklama { get; set; }

        [JsonPropertyName("sayfaSayisi")]
        public int SayfaSayisi { get; set; }

        [JsonPropertyName("ortalamaPuan")]
        public double OrtalamaPuan { get; set; }

        [JsonPropertyName("yorumSayisi")]
        public int YorumSayisi { get; set; }
    }
}