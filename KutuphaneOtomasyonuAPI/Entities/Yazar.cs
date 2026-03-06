using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KutuphaneOtomasyonu.API.Entities
{
    public class Yazar
    {
        [Key]
        [Required]
        [Column("yazar_id")]
        public int YazarId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("ad")]
        public string Ad { get; set; }

        [Required]
        [StringLength(100)]
        [Column("soyad")]
        public string Soyad { get; set; }

        [Column("biyografi")]
        public string Biyografi { get; set; } 
        public ICollection<Kitap> Kitaplari { get; set; }
    }
}