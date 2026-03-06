using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Models/AuthResultDto.cs
namespace KutuphaneOtomasyonu.WPF.Models
{
    public class AuthResultDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}