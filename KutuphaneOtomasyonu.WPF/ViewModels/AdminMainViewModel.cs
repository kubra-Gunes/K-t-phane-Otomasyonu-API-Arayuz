// KutuphaneOtomasyonu.WPF.ViewModels/AdminMainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Services;
using KutuphaneOtomasyonu.WPF.Models;

using System.Diagnostics;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AdminMainViewModel : ObservableObject
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
   

        // Yeni Ekleme Komutları
        public ICommand ShowAddBookCommand { get; }
        public ICommand ShowAddCategoryCommand { get; }
        public ICommand ShowAddAuthorCommand { get; }

        //Güncelleme Komutları 
        public ICommand ShowUpdateBookCommand { get; }
        public ICommand ShowUpdateCategoryCommand { get; }
        public ICommand ShowUpdateAuthorCommand { get; }

        //Kullanıcı Yönetimi Komutları
        public ICommand ShowBorrowBookCommand { get; }
        public ICommand ShowReturnBookCommand { get; }
        public ICommand ShowUsersCommand { get; }

        private readonly BookService _bookService;
        private readonly CategoryService _categoryService;
        private readonly AuthorService _authorService;
        private readonly UserService _userService;


        public AdminMainViewModel()
        {
            _bookService = new BookService();
            _categoryService = new CategoryService();
            _authorService = new AuthorService();
            _userService = new UserService();

            ShowBooksCommand = new AsyncRelayCommand(ShowBooks);
            ShowCategoriesCommand = new AsyncRelayCommand(ShowCategories);
            ShowAuthorsCommand = new AsyncRelayCommand(ShowAuthors);
            ShowUsersCommand = new AsyncRelayCommand(ShowUsers);

    
            // Yeni ekleme komutlarını başlat
            ShowAddBookCommand = new RelayCommand(() => CurrentContentViewModel = new AddBookViewModel());
            ShowAddCategoryCommand = new RelayCommand(() => CurrentContentViewModel = new AddCategoryViewModel());
            ShowAddAuthorCommand = new RelayCommand(() => CurrentContentViewModel = new AddAuthorViewModel());

            //Kulalnıcı İşlemleri
            ShowBorrowBookCommand = new RelayCommand(() => CurrentContentViewModel = new BorrowBookViewModel());
            ShowReturnBookCommand = new RelayCommand(() => CurrentContentViewModel = new ReturnBookViewModel()); // Yeni eklendi

            //Güncelleme işlemleri
            ShowUpdateBookCommand = new RelayCommand(ShowUpdateBook);
            ShowUpdateCategoryCommand = new RelayCommand(() => CurrentContentViewModel = UpdateCategoryViewModel);
            ShowUpdateAuthorCommand = new RelayCommand(async () => await ShowUpdateAuthor());

            //ShowBooksCommand.Execute(null); 

        }

        private async Task ShowBooks()
        {
            var bookListVm = new BookListViewModel(ShowBookDetails);
            CurrentContentViewModel = bookListVm;
            await bookListVm.LoadBooks();
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

        private async Task ShowUsers()
        {
            var userListVm = new UserListViewModel(_userService); // Servisi constructor'a enjekte et
            CurrentContentViewModel = userListVm;
            await userListVm.LoadUsers();
        }
        private void ShowUpdateBook()
        {
            CurrentContentViewModel = new UpdateBookViewModel();
        }

        private UpdateCategoryViewModel _updateCategoryViewModel;
        public UpdateCategoryViewModel UpdateCategoryViewModel
        {
            get
            {
                if (_updateCategoryViewModel == null)
                {
                    _updateCategoryViewModel = new UpdateCategoryViewModel();
                }
                return _updateCategoryViewModel;
            }
        }
        
        private UpdateAuthorViewModel _updateAuthorViewModel;
        public UpdateAuthorViewModel UpdateAuthorViewModel
        {
            get
            {
                if (_updateAuthorViewModel == null)
                {
                    _updateAuthorViewModel = new UpdateAuthorViewModel();
                }
                return _updateAuthorViewModel;
            }
        }
        private async Task ShowUpdateAuthor()
        {
            CurrentContentViewModel = UpdateAuthorViewModel;

            if (UpdateAuthorViewModel != null)
            {
                await UpdateAuthorViewModel.LoadAuthors();
            }
        }

    }

}












