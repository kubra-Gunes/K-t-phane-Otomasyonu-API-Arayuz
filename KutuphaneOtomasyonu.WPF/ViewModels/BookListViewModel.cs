// KutuphaneOtomasyonu.WPF.ViewModels/BookListViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class BookListViewModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly Action<BookDto> _showBookDetailsCallback;
        private ObservableCollection<BookDto> _books;
        public ObservableCollection<BookDto> Books { get; set; }

        private BookDto _selectedBook;
        public BookDto SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value) && value != null)
                {
                    // Seçim değiştiğinde ve null olmadığında callback'i çağır
                    _showBookDetailsCallback?.Invoke(value);
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

        // Arama metni için yeni özellik
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    // Arama metni değiştiğinde kitapları yeniden yükle
                    LoadBooksCommand.Execute(null);
                }
            }
        }

        public ICommand LoadBooksCommand { get; }
        public BookListViewModel()
        {
            _bookService = new BookService();
            Books = new ObservableCollection<BookDto>();
            LoadBooksCommand = new AsyncRelayCommand(LoadBooks);
        }

        public BookListViewModel(Action<BookDto> showBookDetailsCallback)
        {
            _bookService = new BookService();
            _showBookDetailsCallback = showBookDetailsCallback;
            Books = new ObservableCollection<BookDto>();
            LoadBooksCommand = new AsyncRelayCommand(LoadBooks);
        }

        public async Task LoadBooks()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var books = await _bookService.GetBooksAsync(SearchText);
                var sortedBooks = books.OrderBy(b => b.KitapAdi).ToList(); 
                Books.Clear();
                foreach (var book in sortedBooks)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kitaplar yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"Kitaplar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadBooksByCategory(string categoryName)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var books = await _bookService.GetBooksByCategoryAsync(categoryName);
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"'{categoryName}' kategorisindeki kitaplar yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"'{categoryName}' kategorisindeki kitaplar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task LoadBooksByAuthorId(int authorId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var books = await _bookService.GetBooksByAuthorIdAsync(authorId);
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yazar ID '{authorId}' için kitaplar yüklenirken hata oluştu: {ex.Message}";
                Console.WriteLine($"Yazar ID '{authorId}' için kitaplar yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}