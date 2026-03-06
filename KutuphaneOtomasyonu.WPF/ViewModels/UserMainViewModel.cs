// KutuphaneOtomasyonu.WPF.ViewModels/UserMainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using KutuphaneOtomasyonu.WPF.ViewModels;
using KutuphaneOtomasyonu.WPF.Views;


namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UserMainViewModel : ObservableObject
    {
        private ObservableObject _currentContentViewModel;
        public ObservableObject CurrentContentViewModel
        {
            get => _currentContentViewModel;
            set => SetProperty(ref _currentContentViewModel, value);
        }

        public ICommand ShowBooksCommand { get; }
        public ICommand ShowCategoriesCommand { get; }
        public ICommand ShowAuthorsCommand { get; }
        public ICommand NavigateToMyCommentsCommand { get; }

        private readonly BookService _bookService;

        public UserMainViewModel()
        {
            _bookService = new BookService();
            CurrentContentViewModel = new PlaceholderViewModel("Hoş geldiniz! Sol menüden işlem seçiniz.");

            ShowBooksCommand = new AsyncRelayCommand(ShowBooks);
            ShowCategoriesCommand = new AsyncRelayCommand(ShowCategories);
            ShowAuthorsCommand = new AsyncRelayCommand(ShowAuthors);
            NavigateToMyCommentsCommand = new RelayCommand(NavigateToMyComments);

        }

        private void NavigateToMyComments(object obj)
        {
            // MyCommentsViewModel'e, 'string' tipinde bir parametre alan yeni metodu gönderiyoruz.
            var myCommentsViewModel = new MyCommentsViewModel(_bookService, NavigateToBookDetail);
            CurrentContentViewModel = myCommentsViewModel;
            myCommentsViewModel.LoadCommentsCommand.Execute(null);
        }

        // Bu, MyCommentsViewModel'den gelen 'string kitapId'yi işleyecek yeni metot
        private async void NavigateToBookDetail(string kitapId)
        {
            // BookService'den kitabı ID'sine göre al
            var book = await _bookService.GetBookDetailsAsync(kitapId);
            if (book != null)
            {
                // Mevcut metodunuzu çağırarak detay sayfasını göster
                ShowBookDetails(book);
            }
        }

        public void ShowBookDetails(BookDto book)
        {
            // 1. Yazar sayfasına navigasyon işlemini tanımla (BookListViewModel'ı yazar ID'si ile filtrele)
            Action<int> navigateToAuthor = async (yazarId) =>
            {
                var bookListVm = new BookListViewModel(ShowBookDetails); // Kitap listesinden detay sayfasına dönebilmek için callback'i veriyoruz
                CurrentContentViewModel = bookListVm;
                await bookListVm.LoadBooksByAuthorId(yazarId); // Kitap listesini yazar ID'sine göre yükle
            };

            // 2. Kategori sayfasına navigasyon işlemini tanımla (BookListViewModel'ı kategori adına göre filtrele)
            Action<string> navigateToCategory = async (kategoriAdi) =>
            {
                var bookListVm = new BookListViewModel(ShowBookDetails); // Kitap listesinden detay sayfasına dönebilmek için callback'i veriyoruz
                CurrentContentViewModel = bookListVm;
                await bookListVm.LoadBooksByCategory(kategoriAdi); // Kitap listesini kategori adına göre yükle
            };

            // BookDetailViewModel'ı güncellenmiş constructor ile oluştur
            CurrentContentViewModel = new BookDetailViewModel(book, navigateToAuthor, navigateToCategory);
        }

        private async Task ShowBooks()
        {
            var bookListVm = new BookListViewModel(ShowBookDetails);
            CurrentContentViewModel = bookListVm;
            await bookListVm.LoadBooks();
        }

        private async Task ShowCategories()
        {
            // Kategoriye tıklandığında çalışacak EYLEMİ tanımla. Artık ikinci bir callback de alacak.
            Action<string, Action<BookDto>> showBooksAction = async (categoryName, showBookDetailsCallback) =>
            {
                // BookListViewModel'ı oluştururken, detay açma callback'ini parametre olarak veriyoruz
                var bookListVm = new BookListViewModel(showBookDetailsCallback);
                CurrentContentViewModel = bookListVm;
                await bookListVm.LoadBooksByCategory(categoryName);
            };

            // CategoryListViewModel'ı güncellenmiş constructor ile oluştur
            var categoryListVm = new CategoryListViewModel(showBooksAction, ShowBookDetails);
            CurrentContentViewModel = categoryListVm;
            await categoryListVm.LoadCategories();
        }
        private async Task ShowAuthors()
        {
            // Yazara tıklandığında çalışacak EYLEMİ tanımla. Artık ikinci bir callback de alacak.
            Func<int, Action<BookDto>, Task> onAuthorSelected = async (authorId, showBookDetailsCallback) =>
            {
                // BookListViewModel'ı oluştururken, detay açma callback'ini parametre olarak veriyoruz
                var bookListVm = new BookListViewModel(showBookDetailsCallback);
                CurrentContentViewModel = bookListVm;
                await bookListVm.LoadBooksByAuthorId(authorId);
            };

            // AuthorListViewModel'ı güncellenmiş constructor ile oluştur
            var authorListVm = new AuthorListViewModel(onAuthorSelected, ShowBookDetails);
            CurrentContentViewModel = authorListVm;
            await authorListVm.LoadAuthors();
        }
    }
        public class PlaceholderViewModel : ObservableObject
    {
        public string Message { get; set; }
        public PlaceholderViewModel(string message)
        {
            Message = message;
        }
    }
}