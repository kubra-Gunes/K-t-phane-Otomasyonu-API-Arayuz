// KutuphaneOtomasyonu.WPF.ViewModels/UpdateBookViewModel.cs
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;


namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UpdateBookViewModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly CategoryService _categoryService;

        private ObservableCollection<BookDto> _books;
        public ObservableCollection<BookDto> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        private BookDto _selectedBook;
        public BookDto SelectedBook
        {
            get => _selectedBook;
            set
            {
                SetProperty(ref _selectedBook, value);
                if (value != null)
                {
                    // BookDto.KitapId string olduğu için, LoadBookDetails metoduna doğrudan string olarak gönderiyoruz.
                    LoadBookDetails(value.KitapId);
                }
            }
        }

        private UpdateBookDto _updatedBook;
        public UpdateBookDto UpdatedBook
        {
            get => _updatedBook;
            set => SetProperty(ref _updatedBook, value);
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

        public ICommand UpdateBookCommand { get; }
        public ICommand LoadBooksCommand { get; }

        public ICommand DeleteBookCommand { get; }
        public UpdateBookViewModel()
        {
            _bookService = new BookService();
            _authorService = new AuthorService();
            _categoryService = new CategoryService();

            UpdatedBook = new UpdateBookDto();
            Books = new ObservableCollection<BookDto>();
            Authors = new ObservableCollection<AuthorDto>();
            Categories = new ObservableCollection<CategoryDto>();

            UpdateBookCommand = new AsyncRelayCommand(OnUpdateBook, CanUpdateBook);
            LoadBooksCommand = new AsyncRelayCommand(LoadBooks);
            DeleteBookCommand = new AsyncRelayCommand(OnDeleteBook, CanDeleteBook);

            InitializeData();
        }

        private async void InitializeData()
        {
            await LoadBooks();
            await LoadAuthors();
            await LoadCategories();
        }

        public async Task LoadBooks()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var books = await _bookService.GetBooksAsync();
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kitaplar yüklenirken bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error loading books: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadAuthors()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var authors = await _authorService.GetAuthorsAsync();
                Authors.Clear();
                foreach (var author in authors)
                {
                    Authors.Add(author);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yazarlar yüklenirken bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error loading authors: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCategories()
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

        // bookIdString artık BookDto.KitapId'den gelen GUID string'i temsil ediyor.
        private async void LoadBookDetails(string bookIdString)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                // GUID string'i doğrudan kullanıyoruz, int'e çevirmeye gerek yok
                var book = await _bookService.GetBookByIdAsync(bookIdString); // API servisi de string ID almalı
                if (book != null)
                {
                    UpdatedBook = new UpdateBookDto
                    {
                        KitapId = book.KitapId, // BookDto.KitapId (string) -> UpdatedBookDto.KitapId (string)
                        KitapAdi = book.KitapAdi,
                        YazarId = book.YazarId,
                        KategoriId = book.KategoriId,
                        StokAdedi = book.StokAdedi,
                        MevcutAdet = book.MevcutAdet,
                        YayinYili = book.YayinYili,
                        Aciklama = book.Aciklama,
                        SayfaSayisi = book.SayfaSayisi
                    };
                }
                else
                {
                    ErrorMessage = "Kitap bulunamadı.";
                    UpdatedBook = new UpdateBookDto(); // Formu temizle
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kitap detayları yüklenirken bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error loading book details: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanUpdateBook()
        {
            // Güncelleme butonu, UpdatedBook null değilse ve temel alanlar doluysa aktif olsun.
            // KitapId'nin boş veya geçersiz bir GUID olup olmadığını kontrol edelim
            return UpdatedBook != null &&
                   !string.IsNullOrWhiteSpace(UpdatedBook.KitapId) && // KitapId artık string
                   Guid.TryParse(UpdatedBook.KitapId, out _) && // Geçerli bir GUID olup olmadığını kontrol et
                   !string.IsNullOrWhiteSpace(UpdatedBook.KitapAdi) &&
                   UpdatedBook.YazarId > 0 &&
                   UpdatedBook.KategoriId > 0 &&
                   UpdatedBook.StokAdedi >= 0 &&
                   UpdatedBook.YayinYili > 0 &&
                   !IsLoading;
        }

        private async Task OnUpdateBook()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            try
            {
                // KitapId'nin boş veya geçersiz bir GUID olup olmadığını tekrar kontrol et
                if (string.IsNullOrWhiteSpace(UpdatedBook.KitapId) || !Guid.TryParse(UpdatedBook.KitapId, out _))
                {
                    ErrorMessage = "Lütfen güncellenecek geçerli bir kitap seçin.";
                    return;
                }

                if (UpdatedBook.MevcutAdet > UpdatedBook.StokAdedi)
                {
                    ErrorMessage = "Mevcut adet, stok adetinden fazla olamaz.";
                    return;
                }

                bool result = await _bookService.UpdateBookAsync(UpdatedBook);

                if (result)
                {
                    SuccessMessage = "Kitap başarıyla güncellendi!";
                    await LoadBooks(); // Kitap listesini yenile
                    SelectedBook = null; // Seçimi temizle
                    UpdatedBook = new UpdateBookDto(); // Formu temizle
                }
                else
                {
                    ErrorMessage = _bookService.ErrorMessage ?? "Kitap güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error updating book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanDeleteBook()
        {
            // Kitap seçilmişse ve yükleme durumu yoksa silme butonu aktif olsun.
            return SelectedBook != null && !IsLoading;
        }

        private async Task OnDeleteBook()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedBook == null)
            {
                ErrorMessage = "Lütfen silmek istediğiniz kitabı seçin.";
                IsLoading = false;
                return;
            }

            MessageBoxResult dialogResult = MessageBox.Show(
                $"{SelectedBook.KitapAdi} adlı kitabı silmek istediğinizden emin misiniz?",
                "Kitap Sil",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (dialogResult != MessageBoxResult.Yes)
            {
                IsLoading = false;
                return;
            }

            try
            {
                bool deleteSuccess = await _bookService.DeleteBookAsync(SelectedBook.KitapId);

                if (deleteSuccess)
                {
                    SuccessMessage = $"{SelectedBook.KitapAdi} adlı kitap başarıyla silindi!";
                    await LoadBooks(); // Kitap listesini yenile
                    SelectedBook = null; // Seçimi temizle
                    UpdatedBook = new UpdateBookDto(); // Formu temizle
                }
                else
                {
                    ErrorMessage = _bookService.ErrorMessage ?? "Kitap silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                Console.WriteLine($"Error deleting book: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}