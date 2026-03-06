// KutuphaneOtomasyonu.WPF.ViewModels/AdminLoginViewModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security; // SecureString için
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // ICommand için
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Services;
using KutuphaneOtomasyonu.WPF.Models; // AuthResultDto için

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class AdminLoginViewModel : ObservableObject
    {
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private readonly AuthService _authService;

        public ICommand LoginCommand { get; }

        public AdminLoginViewModel()
        {
            _authService = new AuthService(); 
            LoginCommand = new AsyncRelayCommand(Login); 
        }

        private async Task Login()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "E-posta ve şifre boş bırakılamaz.";
                return;
            }

            AuthResultDto authResult = await _authService.LoginAsync(Email, Password);

            if (authResult.IsSuccess)
            {
                if (authResult.Role == "Admin")
                {
                    BaseApiService.CurrentUserToken = authResult.Token; 
                    ErrorMessage = "Giriş başarılı! Yönetici Paneline Yönlendiriliyorsunuz...";
                    OnLoginSuccess?.Invoke(this, authResult.Token); 
                }
                else
                {
                    ErrorMessage = "Yönetici girişi sadece yöneticiler içindir.";
                }
            }
            else
            {
                ErrorMessage = authResult.ErrorMessage ?? "Giriş başarısız.";
            }
        }

        // Giriş başarılı olduğunda View'a bildirmek için event
        public event EventHandler<string> OnLoginSuccess;
    }

    // Async komutları yönetmek için basit bir RelayCommand türevi
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute());
        }

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            CommandManager.InvalidateRequerySuggested();
            try
            {
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}