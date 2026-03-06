// KutuphaneOtomasyonu.WPF.ViewModels/ReturnBookViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class ReturnBookViewModel : ObservableObject
    {
        private ObservableCollection<OduncIslemDto> _borrowedBooks; // Ödünç alınan kitapları tutacak
        public ObservableCollection<OduncIslemDto> BorrowedBooks
        {
            get => _borrowedBooks;
            set => SetProperty(ref _borrowedBooks, value);
        }

        private OduncIslemDto _selectedBorrowedBook; // Seçilen ödünç işlemi
        public OduncIslemDto SelectedBorrowedBook
        {
            get => _selectedBorrowedBook;
            set => SetProperty(ref _selectedBorrowedBook, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadBorrowedBooksCommand { get; }
        public ICommand ReturnBookCommand { get; }


        private readonly BorrowService _borrowService;

        public ReturnBookViewModel()
        {
            _borrowService = new BorrowService();
            BorrowedBooks = new ObservableCollection<OduncIslemDto>();

            LoadBorrowedBooksCommand = new AsyncRelayCommand(LoadBorrowedBooks);
            ReturnBookCommand = new AsyncRelayCommand(ReturnBook);
 
            LoadBorrowedBooksCommand.Execute(null);
        }



        private async Task LoadBorrowedBooks()
        {
            StatusMessage = "Ödünç alınan kitaplar yükleniyor...";
            var borrowed = await _borrowService.GetAllBorrowOperationsAsync(); 

            if (borrowed != null)
            {
                BorrowedBooks.Clear();
                foreach (var item in borrowed.Where(x => !x.TeslimEdildi)) 
                {
                    item.IsOverdue = DateTime.Now > item.SonTeslimTarihi;
                    BorrowedBooks.Add(item);
                }
                StatusMessage = BorrowedBooks.Any() ? "Ödünç alınan kitaplar yüklendi." :
                                                      "Şu anda ödünç alınmış kitap bulunmamaktadır.";
            }
            else
            {
                System.Windows.MessageBox.Show(_borrowService.ErrorMessage, "Hata Detayı");
                Console.WriteLine($"Hata Detayı: {_borrowService.ErrorMessage}"); // Output penceresinde görmek için
                StatusMessage = "Ödünç alınan kitaplar yüklenirken bir hata oluştu.";
            }
        }

        private async Task ReturnBook()
        {
            if (SelectedBorrowedBook == null)
            {
                StatusMessage = "Lütfen iade etmek istediğiniz ödünç işlemi seçin.";
                return;
            }

            StatusMessage = "İade işlemi başlatılıyor...";
            var success = await _borrowService.ReturnBookAsync(SelectedBorrowedBook.OduncIslemId);

            if (success)
            {
                StatusMessage = $"{SelectedBorrowedBook.KitapAdi} kitabı başarıyla iade edildi.";
                // Listeyi güncellemek için yeniden yükle
                await LoadBorrowedBooks();
                SelectedBorrowedBook = null; // Seçimi temizle
            }
            else
            {
                StatusMessage = _borrowService.ErrorMessage ?? "Kitap iade edilirken bir hata oluştu.";
            }
        }



        // OduncIslemDto modelini tanımlıyoruz (API'deki DTO ile eşleşmeli)
        //public class OduncIslemDto
        //{
        //    public int OduncIslemId { get; set; } // API'den gelen ID (int tipinde)
        //    public int KullaniciId { get; set; }
        //    public string KullaniciEmail { get; set; } // Yeni eklendi
        //    public string KitapId { get; set; }
        //    public string KitapAdi { get; set; } // Görüntüleme için
        //    public DateTime OduncAlmaTarihi { get; set; }
        //    public DateTime SonTeslimTarihi { get; set; }
        //    public bool TeslimEdildi { get; set; }
        //    public DateTime? TeslimTarihi { get; set; }
        //}
    }
}