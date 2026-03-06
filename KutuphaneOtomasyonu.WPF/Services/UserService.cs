// KutuphaneOtomasyonu.WPF.Services/UserService.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization; // JsonPropertyName için
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models; // UserDto ve OduncIslemDto için

namespace KutuphaneOtomasyonu.WPF.Services
{
    public class UserService : BaseApiService
    {
        public bool IsLoading { get; set; }
        public string ErrorMessage { get; set; }

        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7230/api/";

        public UserService() : base()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(CurrentUserToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentUserToken);
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync("Kullanicilar");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var usersWrapper = JsonSerializer.Deserialize<JsonListWrapper<UserDto>>
                (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return usersWrapper?.Values ?? new List<UserDto>();
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"User Service Network Error: {httpEx.Message}");
                return null;
            }
            catch (JsonException jsonEx)
            {
                ErrorMessage = $"Veri formatı hatası (JSON): {jsonEx.Message}";
                Console.WriteLine($"User Service JSON Error: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kullanıcılar getirilirken beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"User Service General Error: {ex.Message}");
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<IEnumerable<OduncIslemDto>> GetBorrowRecordsByUserIdAsync(int userId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"OduncIslemleri/byUser/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var wrapper = JsonSerializer.Deserialize<JsonListWrapper<OduncIslemDto>>
                    (content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return wrapper?.Values ?? new List<OduncIslemDto>();
                }
                else
                {
                    ErrorMessage = $"Kullanıcının ödünç kayıtları yüklenirken hata oluştu: {response.ReasonPhrase}";
                    var errorContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        ErrorMessage += $" Detay: {errorContent}";
                    }
                    Console.WriteLine($"API Error: {ErrorMessage}");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"User Service Borrow Records Network Error: {httpEx.Message}");
                return null;
            }
            catch (JsonException jsonEx)
            {
                ErrorMessage = $"Ödünç kayıtları getirilirken veri formatı hatası (JSON): {jsonEx.Message}";
                Console.WriteLine($"User Service Borrow Records JSON Error: {jsonEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ödünç kayıtları getirilirken beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"User Service Borrow Records General Error: {ex.Message}");
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<UserDto> AddUserAsync(RegisterUserDto newUser)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("Kullanicilar/register", newUser);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kullanıcı eklenirken hata oluştu: {ex.Message}";
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<ObservableCollection<BookCommentDto>> GetUserCommentsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"api/KitapYorumlari/GetUserComments/{userId}");
            response.EnsureSuccessStatusCode();
            var comments = await response.Content.ReadAsAsync<IEnumerable<BookCommentDto>>();
            return new ObservableCollection<BookCommentDto>(comments);
        }

        public async Task DeleteCommentAsync(string commentId)
        {
            var response = await _httpClient.DeleteAsync($"api/KitapYorumlari/{commentId}");
            response.EnsureSuccessStatusCode();
        }
    }

   

    public class RegisterUserDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}