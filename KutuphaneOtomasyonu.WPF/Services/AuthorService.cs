using System; // Console.WriteLine için
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text; // StringContent için
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;

namespace KutuphaneOtomasyonu.WPF.Services
{
    public class AuthorService : BaseApiService
    {


        public AuthorService() : base()
        {

        }

        public async Task<List<AuthorDto>> GetAuthorsAsync(string searchText = null)
        {
            try
            {
                AddAuthorizationHeader(); 
                string requestUrl = "Yazarlar";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    requestUrl += $"?searchText={Uri.EscapeDataString(searchText)}";
                }
                var response = await _httpClient.GetAsync(requestUrl);
                Debug.WriteLine($"AuthorService: HTTP Response Status Code: {response.StatusCode}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"AuthorService: Raw API Response Content: {responseContent}");

                var jsonDocument = JsonDocument.Parse(responseContent);
                if (jsonDocument.RootElement.TryGetProperty("$values", out JsonElement valuesElement))
                {
                    var authors = JsonSerializer.Deserialize<List<AuthorDto>>(valuesElement.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return authors;
                }
                return new List<AuthorDto>();
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Debug.WriteLine($"AuthorService Network Error: {ex.Message}");
                return new List<AuthorDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"AuthorService General Error: {ex.Message}");
                return new List<AuthorDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }




  
        public async Task<bool> UpdateAuthorAsync(UpdateAuthorDto updatedAuthor)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var json = JsonSerializer.Serialize(updatedAuthor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Yazarlar/{updatedAuthor.YazarId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Yazar güncellenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; }
        }

        // Yeni Ekleme Metodu
        public async Task<bool> CreateAuthorAsync(CreateAuthorDto newAuthor)
        {
            try
            {
                AddAuthorizationHeader(); // Yetkilendirme başlığını ekle
                var json = JsonSerializer.Serialize(newAuthor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Yazarlar", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Yazar eklenirken hata oluştu: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ağ hatası oluştu: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Beklenmeyen bir hata oluştu: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                AddAuthorizationHeader(); // Yetkilendirme başlığını ekle
                var response = await _httpClient.DeleteAsync($"Yazarlar/{authorId}"); // HTTP DELETE isteği

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Yazar silinirken hata oluştu: {response.StatusCode} - {errorContent}";
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}










