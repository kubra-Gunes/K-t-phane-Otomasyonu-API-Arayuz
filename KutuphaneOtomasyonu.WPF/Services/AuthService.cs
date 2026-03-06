using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Models;

namespace KutuphaneOtomasyonu.WPF.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7230/api/";

        public AuthService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password)
        {
            var loginData = new LoginUserDto { Email = email, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Kullanicilar/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent);
                    return new AuthResultDto
                    {
                        Token = loginResponse?.Token,
                        Role = loginResponse?.Rol,
                        IsSuccess = true
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login Error: {response.StatusCode} - {errorContent}");
                    return new AuthResultDto
                    {
                        IsSuccess = false,
                        ErrorMessage = "Geçersiz e-posta veya şifre."
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network Error: {ex.Message}");
                return new AuthResultDto
                {
                    IsSuccess = false,
                    ErrorMessage = $"Ağ hatası: {ex.Message}"
                };
            }
        }
    }
}