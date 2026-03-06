using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/YazarDto.cs
namespace KutuphaneOtomasyonu.WPF.Models
{
    public class AuthorDto
    {
        public int YazarId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Biyografi { get; set; }
        public string TamAd
        {
            get { return $"{Ad} {Soyad}"; }
        }
    }
}