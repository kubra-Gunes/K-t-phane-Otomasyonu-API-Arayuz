using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text; // StringContent için
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Models; // CategoryDto ve CreateCategoryDto için

namespace KutuphaneOtomasyonu.WPF.Services
{
    public class CategoryService : BaseApiService
    {
        private readonly HttpClient _httpClient;
        // API'nizin temel URL'sini buraya doğru şekilde yazmalısınız.        private const string BaseUrl = "https://localhost:7230/api/";

        public CategoryService() : base()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            Debug.WriteLine("CategoryService: GetCategoriesAsync başlatıldı.");
            IsLoading = true; 
            ErrorMessage = string.Empty; 

            try
            {
                var response = await _httpClient.GetAsync("Kategoriler");
                Debug.WriteLine($"CategoryService: GetCategoriesAsync - HTTP Status Kodu: {response.StatusCode}"); 
                response.EnsureSuccessStatusCode(); 

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"CategoryService: GetCategoriesAsync - Yanıt İçeriği: {jsonResponse}"); 
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var rootObject = JsonDocument.Parse(jsonResponse);

                if (rootObject.RootElement.TryGetProperty("$values", out var valuesElement))
                {
                    var categories = valuesElement.Deserialize<IEnumerable<CategoryDto>>(options);
                    return categories;
                }
                else
                {
                    var categories = JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(jsonResponse, options);
                    return categories;
                }
            }
            catch (HttpRequestException httpEx)
            {
                // HTTP istekleriyle ilgili hataları yakala
                ErrorMessage = $"API isteği sırasında hata oluştu: {httpEx.Message}";
                Debug.WriteLine($"CategoryService: GetCategoriesAsync - Ağ Hatası: {httpEx.Message}"); 
                Console.WriteLine($"HTTP Hatası: {httpEx.Message}");
                return new List<CategoryDto>(); // Boş liste döndür
            }
            catch (JsonException jsonEx)
            {
                // JSON ayrıştırma hatalarını yakala
                ErrorMessage = $"API yanıtı ayrıştırılırken hata oluştu: {jsonEx.Message}";
                Console.WriteLine($"JSON Ayrıştırma Hatası: {jsonEx.Message}");
                return new List<CategoryDto>(); // Boş liste döndür
            }
            catch (Exception ex)
            {
                // Beklenmeyen genel hataları yakala
                ErrorMessage = $"Kategoriler yüklenirken beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Genel Hata: {ex.Message}");
                return new List<CategoryDto>(); // Boş liste döndür
            }
            finally
            {
                IsLoading = false; // Yükleme durumunu sonlandır
                Debug.WriteLine("CategoryService: GetCategoriesAsync tamamlandı.");
            }
        }

        public async Task<bool> CreateCategoryAsync(CreateCategoryDto newCategory)
        {
            try
            {
                AddAuthorizationHeader(); // Yetkilendirme başlığını ekle
                var json = JsonSerializer.Serialize(newCategory);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Kategoriler", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kategori eklenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"Kategori Ekleme Hatası: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Console.WriteLine($"Network Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Genel Hata: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto updatedCategory)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var jsonContent = JsonSerializer.Serialize(updatedCategory);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // PUT isteği gönderiyoruz. API'deki adres /api/Kategoriler/{id} şeklinde olduğu için ID'yi URL'ye ekliyoruz.
                var response = await _httpClient.PutAsync($"Kategoriler/{updatedCategory.KategoriId}", content);
                Debug.WriteLine($"CategoryService: UpdateCategoryAsync - HTTP Status Kodu: {response.StatusCode}"); // YENİ EKLE

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("CategoryService: UpdateCategoryAsync - Kategori başarıyla güncellendi."); // YENİ EKLE
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kategori güncellenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine($"CategoryService: UpdateCategoryAsync - Hata: {response.StatusCode} - {errorContent}"); // YENİ EKLE
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Debug.WriteLine($"CategoryService: UpdateCategoryAsync - Ağ Hatası: {ex.Message}"); // YENİ EKLE
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"CategoryService: UpdateCategoryAsync - Genel Hata: {ex.Message}"); // YENİ EKLE
                return false;
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("CategoryService: UpdateCategoryAsync tamamlandı."); // YENİ EKLE
            }
        }
        // KutuphaneOtomasyonu.WPF.Services/CategoryService.cs
        // ... (diğer metodlar) ...

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            Debug.WriteLine($"CategoryService: DeleteCategoryAsync - ID: {id} başlatıldı."); // Debug Log

            try
            {
                 AddAuthorizationHeader(); 
                var response = await _httpClient.DeleteAsync($"Kategoriler/{id}");
                Debug.WriteLine($"CategoryService: DeleteCategoryAsync - HTTP Status Kodu: {response.StatusCode}"); // Debug Log

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"CategoryService: DeleteCategoryAsync - Kategori ID {id} başarıyla silindi."); // Debug Log
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kategori silinirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine($"CategoryService: DeleteCategoryAsync - Hata: {response.StatusCode} - {errorContent}"); // Debug Log
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Debug.WriteLine($"CategoryService: DeleteCategoryAsync - Ağ Hatası: {ex.Message}"); // Debug Log
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"CategoryService: DeleteCategoryAsync - Genel Hata: {ex.Message}"); // Debug Log
                return false;
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("CategoryService: DeleteCategoryAsync tamamlandı."); // Debug Log
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
    }
}