using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AddCategoryViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;

        private CreateCategoryDto _newCategory;
        public CreateCategoryDto NewCategory
        {
            get => _newCategory;
            set => SetProperty(ref _newCategory, value);
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

        public ICommand AddCategoryCommand { get; }

        public AddCategoryViewModel()
        {
            _categoryService = new CategoryService();
            NewCategory = new CreateCategoryDto(); // Yeni bir boş kategori DTO'su oluştur
            AddCategoryCommand = new AsyncRelayCommand(AddCategory);
        }

        private async Task AddCategory()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(NewCategory.KategoriAdi))
                {
                    ErrorMessage = "Kategori adı boş bırakılamaz.";
                    return;
                }

                bool result = await _categoryService.CreateCategoryAsync(NewCategory);

                if (result)
                {
                    SuccessMessage = "Kategori başarıyla eklendi!";
                    NewCategory = new CreateCategoryDto();
                }
                
                else
                {ErrorMessage = _categoryService.ErrorMessage ?? "Kategori eklenirken bir hata oluştu.";}
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error adding category: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}