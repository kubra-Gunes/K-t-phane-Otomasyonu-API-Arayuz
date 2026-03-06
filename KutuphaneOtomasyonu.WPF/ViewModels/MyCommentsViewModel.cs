// KutuphaneOtomasyonu.WPF.ViewModels/MyCommentsViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class MyCommentsViewModel : ObservableObject
    {
        private readonly BookService _bookService;

        private ObservableCollection<BookCommentDto> _myComments;
        public ObservableCollection<BookCommentDto> MyComments
        {
            get => _myComments;
            set => SetProperty(ref _myComments, value);
        }

        private BookCommentDto _selectedComment;
        public BookCommentDto SelectedComment
        {
            get => _selectedComment;
            set => SetProperty(ref _selectedComment, value);
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

        // Komutlar
        public ICommand LoadCommentsCommand { get; }
        public ICommand EditCommentCommand { get; }
        public ICommand DeleteCommentCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }

        // Bu komut, bir yorumun ait olduğu kitabın detay sayfasını açmak için kullanılacak.
        private readonly Action<string> _showBookDetailCallback;

        public MyCommentsViewModel(BookService bookService, Action<string> showBookDetailCallback)
        {
            _bookService = bookService;
            _showBookDetailCallback = showBookDetailCallback;
            MyComments = new ObservableCollection<BookCommentDto>();
            LoadCommentsCommand = new AsyncRelayCommand(LoadMyCommentsAsync);

            EditCommentCommand = new RelayCommand(EditComment);
            DeleteCommentCommand = new AsyncRelayCommand(DeleteCommentAsync, CanExecuteEditDelete);

            SaveEditCommand = new AsyncRelayCommand(SaveEditedCommentAsync, CanExecuteEditDelete);
            CancelEditCommand = new RelayCommand(CancelEdit);
        }

        public async Task LoadMyCommentsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var comments = await _bookService.GetMyCommentsAsync();
                MyComments.Clear();
                foreach (var comment in comments)
                {
                    MyComments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Yorumlarınız yüklenirken bir hata oluştu: {ex.Message}";
                Debug.WriteLine($"EXCEPTION: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private void EditComment(object parameter)
        {
            if (parameter is BookCommentDto selected && selected != null)
            {
                SelectedComment = selected;

                // Popup'ı aç
                IsEditPopupVisible = true;
            }
        }



        private async Task DeleteCommentAsync()
        {
            if (SelectedComment == null)
            {
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            // Kullanıcıdan onay al
            if (MessageBox.Show("Seçili yorumu silmek istediğinizden emin misiniz?", "Yorum Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var isSuccess = await _bookService.DeleteCommentAsync(SelectedComment.Id);
                if (isSuccess)
                {
                    // Başarılı olursa, listeden yorumu kaldır ve bir mesaj göster.
                    MyComments.Remove(SelectedComment);
                    MessageBox.Show("Yorum başarıyla silindi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Hata olursa, hata mesajını göster.
                    ErrorMessage = _bookService.ErrorMessage;
                }
            }

            IsLoading = false;
        }
        private async Task SaveEditedCommentAsync()
        {
            if (SelectedComment == null)
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                bool isSuccess = await _bookService.UpdateCommentAsync(SelectedComment.Id, SelectedComment);


                if (isSuccess)
                {
                    MessageBox.Show("Yorum başarıyla güncellendi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsEditPopupVisible = false;
                }
                else
                {
                    ErrorMessage = _bookService.ErrorMessage ?? "Yorum güncellenemedi.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Hata: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CancelEdit()
        {
            IsEditPopupVisible = false;
        }

        private bool CanExecuteEditDelete()
        {
            return SelectedComment != null;
        }

        private bool _isEditPopupVisible;
        public bool IsEditPopupVisible
        {
            get => _isEditPopupVisible;
            set
            {
                _isEditPopupVisible = value;
                OnPropertyChanged(); // INotifyPropertyChanged varsa
            }
        }

    }
}