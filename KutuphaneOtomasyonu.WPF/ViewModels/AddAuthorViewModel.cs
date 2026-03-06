using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AddAuthorViewModel : ObservableObject
    {
        private readonly AuthorService _authorService;

        private CreateAuthorDto _newAuthor;
        public CreateAuthorDto NewAuthor
        {
            get => _newAuthor;
            set => SetProperty(ref _newAuthor, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand AddAuthorCommand { get; }

        public AddAuthorViewModel()
        {
            _authorService = new AuthorService();
            NewAuthor = new CreateAuthorDto(); // Yeni bir boş yazar DTO'su oluştur
            AddAuthorCommand = new AsyncRelayCommand(AddAuthor);
        }

        private async Task AddAuthor()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(NewAuthor.Ad) || string.IsNullOrWhiteSpace(NewAuthor.Soyad))
                {
                    ErrorMessage = "Yazar adı ve soyadı boş bırakılamaz.";
                    return;
                }

                bool result = await _authorService.CreateAuthorAsync(NewAuthor);

                if (result)
                {
                    SuccessMessage = "Yazar başarıyla eklendi!";
                    NewAuthor = new CreateAuthorDto(); // Formu temizle
                    // İsteğe bağlı olarak yazar listesini yenileyebilirsiniz
                    // Messenger.Default.Send(new RefreshAuthorsMessage());
                }
                else
                {
                    ErrorMessage = "Yazar eklenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error adding author: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}