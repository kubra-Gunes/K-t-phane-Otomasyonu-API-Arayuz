// KutuphaneOtomasyonu.WPF.Services/BorrowService.cs
using System;
using System.Collections.Generic; // List<T> için
using System.Collections.ObjectModel; // ObservableCollection için
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization; // JsonPropertyName için
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using static KutuphaneOtomasyonu.WPF.ViewModels.ReturnBookViewModel; // OduncIslemDto için

namespace KutuphaneOtomasyonu.WPF.Services
{
    public class BorrowService : BaseApiService
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public BorrowService() : base()
        {
            
        }

        // Kitap Ödünç Alma
        public async Task<bool> BorrowBookAsync(int kullaniciId, string kitapId)
        {
            try
            {
                AddAuthorizationHeader(); // JWT token varsa ekler

                var borrowDto = new
                {
                    KullaniciId = kullaniciId,
                    KitapId = kitapId
                };

                var json = JsonSerializer.Serialize(borrowDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("OduncIslemleri/oduncAl", content);

                if (response.IsSuccessStatusCode)
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap ödünç alınırken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"BorrowService Hatası: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"BorrowService Network Error: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"BorrowService General Error: {ex.Message}");
                return false;
            }
        }

        // Kitap İade Etme
        public async Task<bool> ReturnBookAsync(int oduncIslemId) // oduncIslemId tipi int olarak değiştirildi
        {
            try
            {
                AddAuthorizationHeader(); 

                var returnDto = new
                {
                    OduncIslemId = oduncIslemId 
                };

                var json = JsonSerializer.Serialize(returnDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("OduncIslemleri/iadeEt", content);

                if (response.IsSuccessStatusCode)
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap iade edilirken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"BorrowService Hatası (İade): {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"BorrowService Network Error (İade): {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"BorrowService General Error (İade): {ex.Message}");
                return false;
            }
        }

        // Tüm ödünç işlemlerini getiren metot (iade edilmemişleri filtrelemek için kullanılacak)
        public async Task<ObservableCollection<OduncIslemDto>> GetAllBorrowOperationsAsync()
        {
            string jsonResponse = string.Empty; // Tanımı dışarı al

            try
            {
                AddAuthorizationHeader();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var response = await _httpClient.GetAsync("OduncIslemleri/tumOduncIslemleri");
                response.EnsureSuccessStatusCode();

                jsonResponse = await response.Content.ReadAsStringAsync();

                var wrapper = JsonSerializer.Deserialize<JsonListWrapper<OduncIslemDto>>(jsonResponse, options);

                ErrorMessage = string.Empty;
                return new ObservableCollection<OduncIslemDto>(wrapper.Values);
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"BorrowService Network Error (Get All): {httpEx.Message}");
                return null;
            }
            catch (JsonException jsonEx)
            {
                ErrorMessage = $"Veri ayrıştırma hatası: {jsonEx.Message}. Yanıt: {jsonResponse}";
                Console.WriteLine($"BorrowService JSON Error (Get All): {jsonEx.Message}");
                Console.WriteLine($"Hatalı JSON: {jsonResponse}");
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"BorrowService General Error (Get All): {ex.Message}");
                return null;
            }
        }


        public async Task<bool> BorrowBookByEmailAsync(string kullaniciEmail, string kitapId)
        {
            try
            {
                AddAuthorizationHeader();

                var borrowDto = new
                {
                    KullaniciEmail = kullaniciEmail, 
                    KitapId = kitapId
                };

                var json = JsonSerializer.Serialize(borrowDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("OduncIslemleri/oduncAlByEmail", content);

                if (response.IsSuccessStatusCode)
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Kitap e-posta ile ödünç alınırken hata oluştu: {response.StatusCode} - {errorContent}";
                    Console.WriteLine($"BorrowService Hatası (E-posta ile Ödünç Alma): {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
                Console.WriteLine($"BorrowService Network Error (E-posta ile Ödünç Alma): {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"BorrowService General Error (E-posta ile Ödünç Alma): {ex.Message}");
                return false;
            }
        }


        //GECİKMİŞ ÖDÜNÇ İŞLEMLERİNİ GÖRMEK İÇİN METOD
        //public async Task<ObservableCollection<OduncIslemDto>> GetOverdueBorrowRecordsAsync()
        //{
        //    string jsonString = string.Empty; // Tanımı dışarı al

        //    try
        //    {
        //        AddAuthorizationHeader(); // JWT token varsa ekler

        //        var response = await _httpClient.GetAsync("OduncIslemleri/overdue");
        //        response.EnsureSuccessStatusCode(); // HTTP durum kodu başarılı değilse exception fırlatır

        //        jsonString = await response.Content.ReadAsStringAsync();

        //        // API'den gelen JSON yapısını ayrıştırmak için JsonListWrapper kullan
        //        var wrapper = JsonSerializer.Deserialize<JsonListWrapper<OduncIslemDto>>(jsonString, _options);

        //        ErrorMessage = string.Empty;
        //        return new ObservableCollection<OduncIslemDto>(wrapper.Values); // Values listesini kullan
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        ErrorMessage = $"Ağ hatası oluştu: {httpEx.Message}";
        //        Console.WriteLine($"BorrowService Network Error (Gecikmiş): {httpEx.Message}");
        //        return null;
        //    }
        //    catch (JsonException jsonEx)
        //    {
        //        ErrorMessage = $"Veri ayrıştırma hatası: {jsonEx.Message}. Yanıt: {jsonString}";
        //        Console.WriteLine($"BorrowService JSON Error (Gecikmiş): {jsonEx.Message}");
        //        Console.WriteLine($"Hatalı JSON: {jsonString}");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
        //        Console.WriteLine($"BorrowService General Error: {ex.Message}");
        //        return null;
        //    }
        //}


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    // PropertyChanged çağrısı yoksa UI güncellenmez
                }
            }
        }
    }

    
}