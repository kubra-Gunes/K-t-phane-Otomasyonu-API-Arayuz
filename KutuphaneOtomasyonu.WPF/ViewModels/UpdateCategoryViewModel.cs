// KutuphaneOtomasyonu.WPF.ViewModels/UpdateCategoryViewModel.cs

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers; // ObservableObject için
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System.Windows;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UpdateCategoryViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;

        private ObservableCollection<CategoryDto> _categories;
        public ObservableCollection<CategoryDto> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private CategoryDto _selectedCategory;
        public CategoryDto SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                if (value != null)
                {
                    // Seçilen kategorinin detaylarını UpdatedCategory'ye kopyala
                    UpdatedCategory = new UpdateCategoryDto
                    {
                        KategoriId = value.KategoriId,
                        KategoriAdi = value.KategoriAdi
                    };
                }
                else
                {
                    // Seçim kaldırıldığında formu temizle
                    UpdatedCategory = new UpdateCategoryDto();
                }

                (UpdateCategoryCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
            }
        }

        private UpdateCategoryDto _updatedCategory;
        public UpdateCategoryDto UpdatedCategory
        {
            get => _updatedCategory;
            set => SetProperty(ref _updatedCategory, value);
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

        public ICommand UpdateCategoryCommand { get; }
        public ICommand LoadCategoriesCommand { get; }

        // public ICommand SaveCategoriesCommand { get; } 

        public ICommand DeleteCategoryCommand { get; }

        public UpdateCategoryViewModel()
        {
            _categoryService = new CategoryService();

            Categories = new ObservableCollection<CategoryDto>();
            UpdatedCategory = new UpdateCategoryDto(); // Başlangıçta boş bir DTO

            UpdateCategoryCommand = new AsyncRelayCommand(OnUpdateCategory, CanUpdateCategory);
            LoadCategoriesCommand = new AsyncRelayCommand(LoadCategories);
            DeleteCategoryCommand = new AsyncRelayCommand(OnDeleteCategory, CanDeleteCategory);

            InitializeData();
        }

        private async void InitializeData()
        {
            await LoadCategories();
        }

        public async Task LoadCategories()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kategoriler yüklenirken bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanUpdateCategory()
        {
            // Güncelleme butonu, UpdatedCategory null değilse, KategoriId geçerliyse
            // ve KategoriAdi boş değilse aktif olsun.
            return UpdatedCategory != null &&
                   UpdatedCategory.KategoriId > 0 &&
                   !string.IsNullOrWhiteSpace(UpdatedCategory.KategoriAdi) &&
                   !IsLoading;
        }

        private async Task OnUpdateCategory()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                if (UpdatedCategory.KategoriId <= 0)
                {
                    ErrorMessage = "Lütfen güncellenecek geçerli bir kategori seçin.";
                    return;
                }

                bool result = await _categoryService.UpdateCategoryAsync(UpdatedCategory);

                if (result)
                {
                    SuccessMessage = "Kategori başarıyla güncellendi!";
                    await LoadCategories(); // Kategori listesini yenile
                    SelectedCategory = null; // Seçimi temizle ve formu sıfırla
                }
                else
                {
                    ErrorMessage = _categoryService.ErrorMessage ?? "Kategori güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error updating category: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private bool CanDeleteCategory()
        {
            // Kategori seçilmişse ve ID'si geçerliyse (sıfırdan büyükse) ve yükleme durumu yoksa silme butonu aktif olsun.
            return SelectedCategory != null && SelectedCategory.KategoriId > 0 && !IsLoading;
        }

        private async Task OnDeleteCategory()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedCategory == null || SelectedCategory.KategoriId <= 0)
            {
                ErrorMessage = "Lütfen silmek istediğiniz kategoriyi seçin.";
                IsLoading = false;
                return;
            }

            // Kullanıcıya emin olup olmadığını sor
            MessageBoxResult result = MessageBox.Show(
                $"{SelectedCategory.KategoriAdi} adlı kategoriyi silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.",
                "Kategori Sil Onayı",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                IsLoading = false;
                return;
            }

            try
            {
                bool deleteResult = await _categoryService.DeleteCategoryAsync(SelectedCategory.KategoriId);

                if (deleteResult)
                {
                    SuccessMessage = $"{SelectedCategory.KategoriAdi} adlı kategori başarıyla silindi!";
                    await LoadCategories(); // Kategori listesini yenile
                    SelectedCategory = null; // Seçimi temizle
                    UpdatedCategory = new UpdateCategoryDto(); // Formu temizle
                }
                else
                {
                    ErrorMessage = _categoryService.ErrorMessage ?? "Kategori silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error deleting category: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}