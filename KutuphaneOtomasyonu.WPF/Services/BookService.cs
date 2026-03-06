using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text; // StringContent için
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models; // BookDto ve CreateBookDto için

namespace KutuphaneOtomasyonu.WPF.Services
{
    // API'den gelen "$values" yapısını karşılamak için yeni bir yardımcı sınıf
    public class ApiResponseWrapper<T>
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        [JsonPropertyName("$values")]
        public List<T> Values { get; set; }
    }

    public class BookService : BaseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options; // _options alanını tanımlayın

        private const string BaseUrl = "https://localhost:7230/api/";

        public BookService() : base()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // JsonSerializerOptions'ı burada başlatın
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // JSON'daki property isimlerini büyük/küçük harf duyarsız eşleştir
            };
        }

        public async Task<IEnumerable<BookDto>> GetBooksAsync(string searchText = null)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                string requestUrl = "Kitaplar";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    requestUrl += $"?searchText={Uri.EscapeDataString(searchText)}";
                }

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<ApiResponseWrapper<BookDto>>(jsonResponse, _options);
                return wrapper?.Values ?? Enumerable.Empty<BookDto>();
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}. Lütfen API'nin çalıştığından emin olun.";
                Console.WriteLine($"Network Error: {ex.Message}");
                return Enumerable.Empty<BookDto>();
            }
            catch (JsonException ex)
            {
                ErrorMessage = $"Veri formatı hatası: {ex.Message}. API'den beklenen formatta veri gelmiyor olabilir.";
                Console.WriteLine($"JSON Error: {ex.Message}");
                return Enumerable.Empty<BookDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"General Error: {ex.Message}");
                return Enumerable.Empty<BookDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<List<BookCommentDto>> GetMyCommentsAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);
                }

                var response = await _httpClient.GetAsync("KitapYorumlari/kullanici");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<BookCommentDto>>(json, _options);
                    Debug.WriteLine($"GetMyCommentsAsync: API'den {apiResponse.Values.Count} yorum geldi.");

                    return apiResponse.Values;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Yorumlar yüklenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine($"GetMyCommentsAsync Hata: {ErrorMessage}");
                    return new List<BookCommentDto>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: GetMyCommentsAsync içindeyken hata: {ex.Message}");
                return new List<BookCommentDto>();
            }
        }
        public async Task<bool> UpdateCommentAsync(int yorumId, BookCommentDto updatedComment)
        {
            try
            {
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);
                }

                var jsonContent = JsonSerializer.Serialize(updatedComment, _options);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"KitapYorumlari/{yorumId}", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Yorum güncellenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine($"UpdateCommentAsync Hata: {ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: UpdateCommentAsync içindeyken hata: {ex.Message}");
                return false;
            }
        }

        
        public async Task<bool> DeleteCommentAsync(int yorumId)
        {
            try
            {
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);
                }

                var response = await _httpClient.DeleteAsync($"KitapYorumlari/{yorumId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Yorum silinirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Debug.WriteLine($"DeleteCommentAsync Hata: {ErrorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: DeleteCommentAsync içindeyken hata: {ex.Message}");
                return false;
            }
        }
        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(string categoryName)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                // API'ye gönderilecek URL'yi YOL PARAMETRESİ olarak değiştirin
                string requestUrl = $"Kitaplar/byCategory/{Uri.EscapeDataString(categoryName)}";
                Debug.WriteLine($"BookService: GetBooksByCategoryAsync isteği gönderiliyor: {BaseUrl}{requestUrl}");

                var response = await _httpClient.GetAsync(requestUrl);

                // HTTP durum kodunu logla
                Debug.WriteLine($"BookService: GetBooksByCategoryAsync yanıt durumu: {response.StatusCode}");

                response.EnsureSuccessStatusCode(); // Başarılı (2xx) HTTP durum kodu gelmezse hata fırlatır

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Gelen JSON yanıtını logla
                Debug.WriteLine($"BookService: Gelen JSON yanıtı: {jsonResponse}");

                // Dönen JSON'ı ApiResponseWrapper ile deserialize et
                var wrapper = JsonSerializer.Deserialize<ApiResponseWrapper<BookDto>>(jsonResponse, _options);
                return wrapper?.Values ?? Enumerable.Empty<BookDto>(); // "$values" listesini döndür
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}. Lütfen API'nin çalıştığından ve '{categoryName}' kategorisi için doğru adrese istek atıldığından emin olun.";
                Debug.WriteLine($"BookService: Ağ Hatası: {ex.Message}"); // Hata mesajını logla
                return Enumerable.Empty<BookDto>();
            }
            catch (JsonException ex) // JSON serileştirme/deserileştirme hatası için
            {
                ErrorMessage = $"Veri formatı hatası: {ex.Message}. API'den beklenen format gelmiyor olabilir.";
                Debug.WriteLine($"BookService: JSON Hatası: {ex.Message}"); // JSON hatasını logla
                return Enumerable.Empty<BookDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"BookService: Genel Hata: {ex.Message}"); // Genel hatayı logla
                return Enumerable.Empty<BookDto>();
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("BookService: GetBooksByCategoryAsync tamamlandı."); // İşlem bitişini logla
            }
        }

        public async Task<List<BookDto>> GetBooksByAuthorIdAsync(int authorId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync($"Kitaplar/byAuthor/{authorId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // API'den gelen "$values" yapısını ele almak için JsonListWrapper kullanın
                var wrapper = JsonSerializer.Deserialize<JsonListWrapper<BookDto>>(responseContent, _options);
                return wrapper.Values;
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Console.WriteLine($"Network Error: {ex.Message}");
                return new List<BookDto>();
            }
            catch (JsonException ex)
            {
                ErrorMessage = $"API yanıtı ayrıştırılırken hata oluştu: {ex.Message}";
                Console.WriteLine($"JSON Ayrıştırma Hatası: {ex.Message}");
                return new List<BookDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Genel Hata: {ex.Message}");
                return new List<BookDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> CreateBookAsync(CreateBookDto newBook)
        { 
            try
            {
                var json = JsonSerializer.Serialize(newBook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Kitaplar", content);

                if (response.IsSuccessStatusCode)
                { return true; }

                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap eklenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"Kitap Ekleme Hatası: {response.StatusCode} - {errorContent}");
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
        public async Task<BookDto> GetBookByIdAsync(string id)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var response = await _httpClient.GetAsync($"Kitaplar/{id}");
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BookDto>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ağ hatası oluştu: {ex.Message}";
                Console.WriteLine($"Network Error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kitap detayları yüklenirken beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"General Error: {ex.Message}");
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> UpdateBookAsync(UpdateBookDto updatedBook)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // API'deki PUT endpoint'i: api/Kitaplar/{id}
                var json = JsonSerializer.Serialize(updatedBook);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Kitaplar/{updatedBook.KitapId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap güncellenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"Kitap Güncelleme Hatası: {response.StatusCode} - {errorContent}");
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
            finally
            {
                IsLoading = false;
            }
        }
        public async Task<bool> DeleteBookAsync(string id)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var response = await _httpClient.DeleteAsync($"Kitaplar/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap silinirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"Kitap Silme Hatası: {response.StatusCode} - {errorContent}");
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
            finally
            {
                IsLoading = false;
            }
        }
        public async Task<IEnumerable<BookCommentDto>> GetCommentsByBookIdAsync(string kitapId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                string requestUrl = $"KitapYorumlari/kitap/{kitapId}";
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                // DEĞİŞİKLİK BURADA: Gelen yanıtı wrapper ile deserialize et
                var wrapper = JsonSerializer.Deserialize<ApiResponseWrapper<BookCommentDto>>(jsonResponse, _options);

                // Wrapper içindeki "$values" listesini döndür
                return wrapper?.Values ?? Enumerable.Empty<BookCommentDto>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yorumlar yüklenirken bir hata oluştu: {ex.Message}";
                Console.WriteLine($"GetCommentsByBookIdAsync Hata: {ex.Message}"); // Debug için Console.WriteLine kullanmak daha iyi olabilir
                return Enumerable.Empty<BookCommentDto>();
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task<BookDto> GetBookDetailsAsync(string bookId)
        {
            try
            {
                if (!string.IsNullOrEmpty(AuthManager.CurrentToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);
                }

                var response = await _httpClient.GetAsync($"Kitaplar/{bookId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<BookDto>>(json, _options);
                    return apiResponse.Values.FirstOrDefault();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap detayları yüklenirken hata oluştu: {response.StatusCode} - {errorContent}";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                return null;
            }
        }
        public async Task<bool> AddCommentAsync(CreateBookCommentDto newComment)
        {
            Debug.WriteLine("BookService.AddCommentAsync calisti.");
            AddAuthorizationHeader();
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var json = JsonSerializer.Serialize(newComment, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthManager.CurrentToken);

                Debug.WriteLine("BookService: Yorum ekleme istegi gonderiliyor.");
                Debug.WriteLine($"BookService: Gonderilen Token: {AuthManager.CurrentToken}");
                var response = await _httpClient.PostAsync("KitapYorumlari", content);

                Debug.WriteLine($"API Yaniti Alindi. StatusCode: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"API HATA icerigi: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yorum eklenirken beklenmeyen bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: AddCommentAsync icinde kritik hata: {ex.Message}");
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
    }
}