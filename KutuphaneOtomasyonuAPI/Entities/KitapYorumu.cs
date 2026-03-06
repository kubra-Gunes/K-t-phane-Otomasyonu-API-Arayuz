// KitapYorumu.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyonu.API.Entities // Kendi namespace'inizi buraya yazın
{
    public class KitapYorumu
    {
        [Key]
        public int Id { get; set; }

        public string KitapId { get; set; }
        [ForeignKey("KitapId")]
        public Kitap Kitap { get; set; } 

        public int KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; } 

        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public int Puan { get; set; }

        [MaxLength(500, ErrorMessage = "Yorum 500 karakterden uzun olamaz.")]
        public string YorumMetni { get; set; } 

        public DateTime YorumTarihi { get; set; } = DateTime.Now;
    }
}