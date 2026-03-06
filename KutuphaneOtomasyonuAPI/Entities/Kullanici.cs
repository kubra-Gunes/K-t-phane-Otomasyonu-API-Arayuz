using System; // DateTime için
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KutuphaneOtomasyonu.API.Entities
{
    public class Kullanici
    {
        [Key]
        [Required]
        [Column("kullanici_id")]
        public int KullaniciId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("ad")]
        public string Ad { get; set; }

        [Required]
        [StringLength(100)]
        [Column("soyad")]
        public string Soyad { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress] // E-posta formatı kontrolü için
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Column("sifre_hash")]
        public string SifreHash { get; set; }

        [Required]
        [StringLength(50)]
        [Column("rol")]
        public string Rol { get; set; } // Örneğin "Admin" veya "Kullanici"

        [Required]
        [Column("kayit_tarihi")]
        public DateTime KayitTarihi { get; set; }

        // İlişkili ödünç kayıtları için navigasyon özelliği
        public ICollection<OduncKayit>? OduncKayitlari { get; set; } // Nullable yapıldı

        public ICollection<KitapYorumu> KitapYorumlari { get; set; }

        public Kullanici()
        {
            OduncKayitlari = new HashSet<OduncKayit>();
            KitapYorumlari = new HashSet<KitapYorumu>(); // Yeni koleksiyonu başlat
        }
    }
}