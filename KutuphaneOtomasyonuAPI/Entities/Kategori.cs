using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneOtomasyonu.API.Entities
{
    public class Kategori
    {
        [Key]
        [Required]
        [Column("kategori_id")]
        public int KategoriId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("kategori_adi")]
        [ConcurrencyCheck] 
        public string KategoriAdi { get; set; }
   
        public ICollection<Kitap> Kitaplari { get; set; }
    }
}