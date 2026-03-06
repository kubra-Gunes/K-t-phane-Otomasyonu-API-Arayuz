using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AddBookViewModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly CategoryService _categoryService;

        private CreateBookDto _newBook;
        public CreateBookDto NewBook
        {
            get => _newBook;
            set => SetProperty(ref _newBook, value);
        }

        private ObservableCollection<AuthorDto> _authors;
        public ObservableCollection<AuthorDto> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        private ObservableCollection<CategoryDto> _categories;
        public ObservableCollection<CategoryDto> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
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

        public ICommand AddBookCommand { get; }
        public AsyncRelayCommand LoadRelatedDataCommand { get; }

        public AddBookViewModel()
        {
            _bookService = new BookService();
            _authorService = new AuthorService();
            _categoryService = new CategoryService();
            NewBook = new CreateBookDto(); // Yeni bir boş kitap DTO'su oluştur
            Authors = new ObservableCollection<AuthorDto>();
            Categories = new ObservableCollection<CategoryDto>();

            AddBookCommand = new AsyncRelayCommand(AddBook);
            LoadRelatedDataCommand = new AsyncRelayCommand(LoadRelatedData);

            // ViewModel yüklendiğinde yazar ve kategori listelerini çek
            _ = LoadRelatedData(); // Async metodları void ile çağırmak yerine _ = ile çağırarak await etmiyoruz.
                                   // Eğer constructor içinde await kullanmak isterseniz async void constructor yapmalısınız,
                                   // ancak bu önerilmez. Genellikle LoadRelatedData gibi metotlar
                                   // View'ın Loaded event'i veya başka bir ICommand ile çağrılır.
        }

        private async Task LoadRelatedData()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                var authors = await _authorService.GetAuthorsAsync();
                if (authors != null)
                {
                    Authors.Clear();
                    foreach (var author in authors)
                    {
                        Authors.Add(author);
                    }
                }
                else
                {
                    ErrorMessage += "Yazarlar yüklenemedi. ";
                }

                var categories = await _categoryService.GetCategoriesAsync();
                if (categories != null)
                {
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }
                else
                {
                    ErrorMessage += "Kategoriler yüklenemedi.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"İlgili veriler yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"Error loading related data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddBook()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                // Basic validation (more robust validation can be added)
                if (string.IsNullOrWhiteSpace(NewBook.KitapAdi) || NewBook.YazarId == 0 || NewBook.KategoriId == 0 || NewBook.StokAdedi < 0 || NewBook.YayinYili == 0)
                {
                    ErrorMessage = "Lütfen tüm gerekli alanları doldurun ve geçerli değerler girin.";
                    return;
                }

                // MevcutAdet alanını StokAdedi ile otomatik doldur (eğer MevcutAdet sıfırsa veya set edilmediyse)
                if (NewBook.MevcutAdet == 0)
                {
                    NewBook.MevcutAdet = NewBook.StokAdedi;
                }

                bool result = await _bookService.CreateBookAsync(NewBook);

                if (result)
                {
                    SuccessMessage = "Kitap başarıyla eklendi!";
                    NewBook = new CreateBookDto(); // Formu temizle
                    // İsteğe bağlı olarak ilgili listeyi yenileyebilirsiniz
                    // Messenger.Default.Send(new RefreshBooksMessage());
                }
                else
                {
                    ErrorMessage = _bookService.ErrorMessage ?? "Kitap eklenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error adding book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}