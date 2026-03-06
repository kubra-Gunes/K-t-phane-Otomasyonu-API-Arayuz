using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System; // DateTime için

namespace KutuphaneOtomasyonu.API.Entities
{
    public class OduncKayit
    {
        [Key]
        [Required]
        [Column("odunc_id")]
        public int OduncId { get; set; }

        [Required]
        [Column("kullanici_id")]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")] // Kullanici sınıfı ile ilişkiyi belirtir
        public Kullanici Kullanici { get; set; } // Navigasyon özelliği

        [Required]
        [Column("kitap_id")]
        public string KitapId { get; set; } // KitapId string olduğu için

        [ForeignKey("KitapId")] // Kitap sınıfı ile ilişkiyi belirtir
        public Kitap Kitap { get; set; } // Navigasyon özelliği

        [Required]
        [Column("odunc_alma_tarihi")]
        public DateTime OduncAlmaTarihi { get; set; }

        [Required]
        [Column("son_teslim_tarihi")]
        public DateTime SonTeslimTarihi { get; set; }

        [Column("iade_tarihi")]
        public DateTime? IadeTarihi { get; set; } // NULL olabileceği için nullable DateTime?

        [Required]
        [StringLength(50)]
        [Column("durum")]
        public string Durum { get; set; } // "OduncAlindi", "IadeEdildi", "Gecikti" gibi
    }
}