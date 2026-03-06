// Kitap.cs dosyanızın içindeki Kitap sınıfı
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KutuphaneOtomasyonu.API.Entities
{
    public class Kitap
    {
        [Key]
        [Required]
        [Column("kitap_id")]
        public string KitapId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("kitap_adi")]
        public string KitapAdi { get; set; }

        [Required]
        [Column("yazar_id")]
        public int YazarId { get; set; }

        [ForeignKey("YazarId")]
        public Yazar Yazar { get; set; }

        [Required]
        [Column("kategori_id")]
        public int KategoriId { get; set; }

        [ForeignKey("KategoriId")]
        public Kategori Kategori { get; set; }

        [Required]
        [Column("stok_adedi")]
        public int StokAdedi { get; set; }

        [Required]
        [Column("mevcut_adet")]
        public int MevcutAdet { get; set; }

        [Required]
        [Column("yayin_yili")]
        public int YayinYili { get; set; }

        [Column("aciklama")]
        public string Aciklama { get; set; }

        // Yeni eklenen sütun
        [Required] // Sayfa sayısı genellikle zorunlu bir bilgi olabilir, duruma göre kaldırabilirsiniz.
        [Column("sayfa_sayisi")]
        public int SayfaSayisi { get; set; } // int türünde bir sütun

        public ICollection<OduncKayit> OduncKayitlari { get; set; }

        public ICollection<KitapYorumu> KitapYorumlari { get; set; }

        public Kitap()
        {
            OduncKayitlari = new HashSet<OduncKayit>();
            KitapYorumlari = new HashSet<KitapYorumu>(); 
        }

    }
}