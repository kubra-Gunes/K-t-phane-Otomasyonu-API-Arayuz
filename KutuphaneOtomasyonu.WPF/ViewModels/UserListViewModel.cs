// KutuphaneOtomasyonu.WPF.ViewModels/UserListViewModel.cs
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Models;
using KutuphaneOtomasyonu.WPF.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls; // PasswordBox için
using System.Windows.Input;
// Using OduncIslemDto for the WPF project (if not already there)
// using KutuphaneOtomasyonu.WPF.Models; // Already present from previous steps.

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UserListViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private bool _isLoading;
        private string _errorMessage;
        private RegisterUserDto _newUser = new RegisterUserDto();

        // YENİ: Seçili kullanıcıyı tutacak özellik
        private UserDto _selectedUser;
        public UserDto SelectedUser
        {
            get => _selectedUser;
            set
            {

                if (SetProperty(ref _selectedUser, value))
                {

                    if (value != null)
                    {

                        _ = LoadUserBorrowRecords(value.Id);
                    }
                    else
                    {

                        UserBorrowRecords.Clear();
                    }
                }
            }
        }

    
        public ObservableCollection<OduncIslemDto> UserBorrowRecords { get; } = new ObservableCollection<OduncIslemDto>();


        public ObservableCollection<UserDto> Users { get; } = new ObservableCollection<UserDto>();

        public RegisterUserDto NewUser
        {
            get => _newUser;
            set => SetProperty(ref _newUser, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand AddUserCommand { get; }



        public UserListViewModel(UserService userService)
        {
            _userService = userService;
            AddUserCommand = new AsyncRelayCommand<PasswordBox>(AddUser);

        }

        // Mevcut LoadUsers metodu (zaten olmalı)
        public async Task LoadUsers()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            Users.Clear();

            var users = await _userService.GetUsersAsync();
            if (users != null)
            {
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            else
            {
                ErrorMessage = "Kullanıcılar yüklenemedi: " + _userService.ErrorMessage;
            }
            IsLoading = false;
        }

        private async Task AddUser(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(NewUser.Ad) ||
                string.IsNullOrWhiteSpace(NewUser.Soyad) ||
                string.IsNullOrWhiteSpace(NewUser.Email) ||
                string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                ErrorMessage = "Tüm alanlar doldurulmalıdır.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            NewUser.Password = passwordBox.Password;

            var addedUser = await _userService.AddUserAsync(NewUser);

            if (addedUser != null)
            {
                await LoadUsers(); // Başarılı ekleme sonrası listeyi yenile
                NewUser = new RegisterUserDto(); // Formu temizle
                passwordBox.Clear();
            }
            else
            {
                ErrorMessage = "Kullanıcı eklenemedi: " + _userService.ErrorMessage;
            }
            IsLoading = false;
        }


        private async Task LoadUserBorrowRecords(int userId)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            UserBorrowRecords.Clear(); // Önceki kayıtları temizle

            var records = await _userService.GetBorrowRecordsByUserIdAsync(userId);
            if (records != null)
            {
                foreach (var record in records)
                {
                    UserBorrowRecords.Add(record);
                }
            }
            else
            {
                ErrorMessage = "Kullanıcının ödünç kayıtları yüklenemedi: " + _userService.ErrorMessage;
            }
            IsLoading = false;
        }
    }
}