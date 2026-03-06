// KutuphaneOtomasyonu.WPF.ViewModels/BorrowBookViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models; // BookDto için
using KutuphaneOtomasyonu.WPF.Services;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class BorrowBookViewModel : ObservableObject
    {
        private ObservableCollection<BookDto> _availableBooks;
        public ObservableCollection<BookDto> AvailableBooks
        {
            get => _availableBooks;
            set => SetProperty(ref _availableBooks, value);
        }

        private BookDto _selectedBook;
        public BookDto SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        private string _kullaniciEmail;
        public string KullaniciEmail
        {
            get => _kullaniciEmail;
            set => SetProperty(ref _kullaniciEmail, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadBooksCommand { get; }
        public ICommand BorrowBookCommand { get; } // Bu komut ödünç alma işlemini tetikleyecek

        private readonly BookService _bookService;
        private readonly BorrowService _borrowService;

        public BorrowBookViewModel()
        {
            _bookService = new BookService();
            _borrowService = new BorrowService();

            LoadBooksCommand = new RelayCommand(async () => await LoadBooks());
            BorrowBookCommand = new RelayCommand(async () => await BorrowBook());

            LoadBooksCommand.Execute(null);
        }

        private async Task LoadBooks()
        {
            StatusMessage = "Kitaplar yükleniyor...";
            var books = await _bookService.GetBooksAsync();
            if (books != null)
            {
                AvailableBooks = new ObservableCollection<BookDto>(books.Where(b => b.MevcutAdet > 0));
                StatusMessage = AvailableBooks.Any() ? "Kitaplar yüklendi." : "Stokta ödünç alınabilecek kitap bulunmamaktadır.";
            }
            else
            {
                StatusMessage = "Kitaplar yüklenirken bir hata oluştu.";
            }
        }

        private async Task BorrowBook()
        {
            if (SelectedBook == null)
            {
                StatusMessage = "Lütfen ödünç almak istediğiniz kitabı seçin.";
                return;
            }
            if (string.IsNullOrWhiteSpace(KullaniciEmail) || !IsValidEmail(KullaniciEmail))
            {
                StatusMessage = "Lütfen geçerli bir kullanıcı e-posta adresi girin.";
                return;
            }
            StatusMessage = "Ödünç alma işlemi başlatılıyor...";
            var success = await _borrowService.BorrowBookByEmailAsync(KullaniciEmail, SelectedBook.KitapId);

            if (success)
            {
                StatusMessage = $"{SelectedBook.KitapAdi} kitabı {KullaniciEmail} e-postalı kullanıcıya ödünç verildi.";
                // Kitap listesini ve stoğu güncellemek için yeniden yükle
                await LoadBooks();
                SelectedBook = null; // Seçimi temizle
                KullaniciEmail = string.Empty; // Kullanıcı e-postasını temizle
            }
            else
            {
                StatusMessage = _borrowService.ErrorMessage ?? "Kitap ödünç alınırken bir hata oluştu.";
            }
        }

        // Basit bir e-posta doğrulama metodu (daha sağlam bir doğrulama kullanabilirsiniz)
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}