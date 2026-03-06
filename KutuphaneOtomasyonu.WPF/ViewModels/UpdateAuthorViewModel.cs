// KutuphaneOtomasyonu.WPF.ViewModels/UpdateAuthorViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System.Windows;


namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UpdateAuthorViewModel : ObservableObject
    {
        private readonly AuthorService _authorService;

        private ObservableCollection<AuthorDto> _authors;
        public ObservableCollection<AuthorDto> Authors
        {
            get => _authors;
            set
            {
                SetProperty(ref _authors, value);
                Debug.WriteLine($"UpdateAuthorViewModel: Authors property set. Current count: {value?.Count}"); // EKLE
            }
        }

        private AuthorDto _selectedAuthor;
        public AuthorDto SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                SetProperty(ref _selectedAuthor, value);
                if (value != null)
                {
                    UpdatedAuthor = new UpdateAuthorDto
                    {
                        YazarId = value.YazarId,
                        Ad = value.Ad,
                        Soyad = value.Soyad,
                        Biyografi = value.Biyografi
                    };
                }
                else
                { UpdatedAuthor = new UpdateAuthorDto(); }
                ((RelayCommand)UpdateAuthorCommand)?.RaiseCanExecuteChanged();
            }
        }

        private UpdateAuthorDto _updatedAuthor;
        public UpdateAuthorDto UpdatedAuthor
        {
            get => _updatedAuthor;
            set
            {
                SetProperty(ref _updatedAuthor, value);
                ((RelayCommand)UpdateAuthorCommand)?.RaiseCanExecuteChanged();
            }
        }

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
            set
            {
                SetProperty(ref _errorMessage, value);
                Debug.WriteLine($"UpdateAuthorViewModel: ErrorMessage set to: {value}"); // EKLE
            }
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                SetProperty(ref _successMessage, value);
                Debug.WriteLine($"UpdateAuthorViewModel: SuccessMessage set to: {value}"); // EKLE
            }
        }

        public ICommand UpdateAuthorCommand { get; }
        public ICommand DeleteAuthorCommand { get; }

        public UpdateAuthorViewModel()
        {
            _authorService = new AuthorService(); 
            UpdatedAuthor = new UpdateAuthorDto();
            UpdateAuthorCommand = new RelayCommand(async () => await OnUpdateAuthor(), CanUpdateAuthor);
            DeleteAuthorCommand = new RelayCommand(async () => await OnDeleteAuthor(), CanDeleteAuthor);
            Debug.WriteLine("UpdateAuthorViewModel: Constructor finished, LoadAuthors called."); // EKLE
        }

        public async Task LoadAuthors()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                Debug.WriteLine("UpdateAuthorViewModel: LoadAuthors started.");
                var fetchedAuthors = await _authorService.GetAuthorsAsync();

                Authors = new ObservableCollection<AuthorDto>(fetchedAuthors);
                Debug.WriteLine("UpdateAuthorViewModel: fetchedAuthors from service. Count: " + fetchedAuthors.Count);

            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yazarlar yüklenirken bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"Error loading authors: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("UpdateAuthorViewModel: LoadAuthors finished. IsLoading: " + IsLoading);
            }
        }

        private bool CanUpdateAuthor()
        {
            return SelectedAuthor != null &&
                   UpdatedAuthor != null &&
                   UpdatedAuthor.YazarId > 0 &&
                   !string.IsNullOrWhiteSpace(UpdatedAuthor.Ad) &&
                   !string.IsNullOrWhiteSpace(UpdatedAuthor.Soyad) &&
                   !IsLoading;
        }

        private async Task OnUpdateAuthor()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                if (UpdatedAuthor.YazarId <= 0)
                {
                    ErrorMessage = "Lütfen güncellenecek geçerli bir yazar seçin.";
                    return;
                }

                bool result = await _authorService.UpdateAuthorAsync(UpdatedAuthor);

                if (result)
                {
                    SuccessMessage = "Yazar başarıyla güncellendi!";
                    await LoadAuthors(); 
                    SelectedAuthor = null; 
                }
                else
                {
                    ErrorMessage = _authorService.ErrorMessage ?? "Yazar güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error updating author: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanDeleteAuthor()
        {
            return SelectedAuthor != null && SelectedAuthor.YazarId > 0 && !IsLoading;
        }
        private async Task OnDeleteAuthor()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                if (SelectedAuthor == null || SelectedAuthor.YazarId <= 0)
                {
                    ErrorMessage = "Lütfen silmek için bir yazar seçin.";
                    return;
                }
                
                if (MessageBox.Show($"{SelectedAuthor.TamAd} adlı yazarı silmek istediğinizden emin misiniz?", 
                "Yazar Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    IsLoading = false;
                    return;
                }

                bool result = await _authorService.DeleteAuthorAsync(SelectedAuthor.YazarId); 

                if (result)
                {
                    SuccessMessage = "Yazar başarıyla silindi!";
                    await LoadAuthors(); 
                    SelectedAuthor = null; 
                }
                else
                {
                    ErrorMessage = _authorService.ErrorMessage ?? "Yazar silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error deleting author: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}





