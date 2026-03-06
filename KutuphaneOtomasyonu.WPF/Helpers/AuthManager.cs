using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneOtomasyonu.WPF.Helpers
{
    // KutuphaneOtomasyonu.WPF/Helpers/AuthManager.cs

    public static class AuthManager
    {
        public static string? CurrentToken { get; private set; }

        public static void SetToken(string token)
        {
            CurrentToken = token;
        }

        public static void ClearToken()
        {
            CurrentToken = null;
        }
    }
}
