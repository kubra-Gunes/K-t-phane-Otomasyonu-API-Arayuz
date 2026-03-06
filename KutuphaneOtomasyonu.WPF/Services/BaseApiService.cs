using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using System.Diagnostics;

namespace KutuphaneOtomasyonu.WPF.Services
{
   
    public abstract class BaseApiService : ObservableObject
    {
        protected readonly HttpClient _httpClient;
        public const string BaseUrl = "https://localhost:7230/api/"; 

        public static string CurrentUserToken { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value); 
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value); 
        }

        public BaseApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        protected void AddAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);

                Debug.WriteLine($"Authorization Header eklendi: Bearer {AuthManager.CurrentToken.Substring(0, 15)}...");
            }
            else
            {
                Debug.WriteLine("Authorization Header EKLENEMEDI! Token bulunamadi.");
            }
        }
    }
}