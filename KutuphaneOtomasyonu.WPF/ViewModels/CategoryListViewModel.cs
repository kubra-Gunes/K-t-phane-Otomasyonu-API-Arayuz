using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands; // AsyncRelayCommand için

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class CategoryListViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;
        private ObservableCollection<CategoryDto> _categories;
        private readonly Action<BookDto> _showBookDetailsCallback;
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
                if (SetProperty(ref _selectedCategory, value))
                {
                    if (_selectedCategory != null)
                    {
                        ShowBooksForCategoryCommand.Execute(_selectedCategory.KategoriAdi);
                    }
                }
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
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoadCategoriesCommand { get; }
        public ICommand ShowBooksForCategoryCommand { get; }

        private readonly Action<string, Action<BookDto>> _showBooksAction;
        public CategoryListViewModel(Action<string, Action<BookDto>> showBooksAction, Action<BookDto> showBookDetailsCallback)
        {
            _categoryService = new CategoryService();
            Categories = new ObservableCollection<CategoryDto>();
            LoadCategoriesCommand = new AsyncRelayCommand(LoadCategories);

            // Yeni callback'i ve action'ı ata
            _showBookDetailsCallback = showBookDetailsCallback;
            _showBooksAction = showBooksAction;

            ShowBooksForCategoryCommand = new AsyncRelayCommand<string>(async (categoryName) =>
            {
                // Artık MainViewModel'a hem kategori adını hem de detay açma callback'ini iletiyoruz
                _showBooksAction?.Invoke(categoryName, _showBookDetailsCallback);
            });
            LoadCategoriesCommand.Execute(null);
        }

        public async Task LoadCategories()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var categories = await _categoryService.GetCategoriesAsync(); 
                var sortedAuthors = categories.OrderBy(a => a.KategoriAdi).ToList();
                Categories.Clear();
                foreach (var author in sortedAuthors)
                {
                    Categories.Add(author);
                }
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kategoriler yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"Kategoriler yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}