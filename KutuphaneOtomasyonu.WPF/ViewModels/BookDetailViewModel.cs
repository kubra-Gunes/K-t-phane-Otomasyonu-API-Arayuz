// KutuphaneOtomasyonu.WPF.ViewModels/BookDetailViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class BookDetailViewModel : ObservableObject
    {
        private readonly BookService _bookService;

        // Yeni: Yazar ve Kategori sayfasına geçiş için callback'ler
        private readonly Action<int> _onNavigateToAuthor;
        private readonly Action<string> _onNavigateToCategory;

        private BookDto _selectedBook;
        public BookDto SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        // ... (Diğer alanlar ve property'ler: Comments, NewCommentRating, NewCommentText vb.)

        private ObservableCollection<BookCommentDto> _comments;
        public ObservableCollection<BookCommentDto> Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }
        private int _newCommentRating;
        private string _newCommentText;

        public int NewCommentRating
        {
            get => _newCommentRating;
            set
            {
                if (SetProperty(ref _newCommentRating, value))
                {
                    (SubmitCommentCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();

                    Debug.WriteLine($"NewCommentRating degeri degisti: {value}");


                }
            }
        }

        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                if (SetProperty(ref _newCommentText, value))
                {
                    // Komutun çalıştırılabilir durumunu yeniden kontrol etmesi için tetikle.
                    (SubmitCommentCommand as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
                    Debug.WriteLine($"NewCommentText degeri degisti: '{value}'");

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

        public ICommand SubmitCommentCommand { get; }
        public ICommand NavigateToAuthorCommand { get; } // Yeni Komut
        public ICommand NavigateToCategoryCommand { get; } // Yeni Komut

        // Constructor güncellendi: Navigasyon callback'leri eklendi
        public BookDetailViewModel(BookDto selectedBook, Action<int> onNavigateToAuthor, Action<string> onNavigateToCategory)
        {
            _bookService = new BookService();
            SelectedBook = selectedBook;
            Comments = new ObservableCollection<BookCommentDto>();
            SubmitCommentCommand = new AsyncRelayCommand(SubmitComment, CanSubmitComment);

            // Callback'leri ata
            _onNavigateToAuthor = onNavigateToAuthor;
            _onNavigateToCategory = onNavigateToCategory;

            // Yeni Komutları tanımla
            NavigateToAuthorCommand = new RelayCommand(ExecuteNavigateToAuthor);
            NavigateToCategoryCommand = new RelayCommand(ExecuteNavigateToCategory);

            // ViewModel oluşturulduğunda yorumları yükle
            LoadComments();
        }

        // Yeni: Yazar sayfasına geçiş metodunun implementasyonu
        private void ExecuteNavigateToAuthor()
        {
            if (SelectedBook != null)
            {
                Debug.WriteLine($"Yazar Sayfasına Navigasyon İsteği: YazarID={SelectedBook.YazarId}");
                // Ana ViewModel'a (veya navigasyon hizmetine) Yazar ID'si ile navigasyon talebini gönder
                _onNavigateToAuthor?.Invoke(SelectedBook.YazarId);
            }
        }

        // Yeni: Kategori sayfasına geçiş metodunun implementasyonu
        private void ExecuteNavigateToCategory()
        {
            if (SelectedBook != null)
            {
                Debug.WriteLine($"Kategori Sayfasına Navigasyon İsteği: KategoriAdi='{SelectedBook.KategoriAdi}'");
                // Ana ViewModel'a (veya navigasyon hizmetine) Kategori Adı ile navigasyon talebini gönder
                _onNavigateToCategory?.Invoke(SelectedBook.KategoriAdi);
            }
        }


        private async Task LoadComments()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var comments = await _bookService.GetCommentsByBookIdAsync(SelectedBook.KitapId);
                Comments.Clear();
                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yorumlar yüklenirken hata: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SubmitComment()
        {
            Debug.WriteLine("SubmitComment metodu basladi.");
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var newComment = new CreateBookCommentDto
                {
                    KitapId = SelectedBook.KitapId,
                    Puan = NewCommentRating,
                    YorumMetni = NewCommentText
                };

                Debug.WriteLine($"Servis cagriliyor: KitapID={newComment.KitapId}, Puan={newComment.Puan}");
                bool success = await _bookService.AddCommentAsync(newComment);
                Debug.WriteLine($"Servis cagrildi. Basari durumu: {success}");
                if (success)
                {
                    Debug.WriteLine("Yorum basariyla eklendi. Liste yenileniyor.");
                    NewCommentText = string.Empty;
                    NewCommentRating = 5; // Varsayılan değere döndür
                    await LoadComments(); // Listeyi güncelle
                }
                else
                {
                    ErrorMessage = _bookService.ErrorMessage;
                    Debug.WriteLine($"HATA: Yorum eklenemedi. Servis Mesaji: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yorum gönderilirken hata: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: SubmitComment icinde kritik hata: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                Debug.WriteLine("SubmitComment metodu tamamlandi.");
            }
        }

        // Yorum gönderme butonunun aktif olup olmayacağını kontrol eder.
        private bool CanSubmitComment()
        {
            bool result = !string.IsNullOrWhiteSpace(NewCommentText) && NewCommentRating >= 1;
            Debug.WriteLine($"CanSubmitComment calisti. Yorum Metni: '{NewCommentText}', Sonuc: {result}");
            return result;
        }
    }
}