
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using KutuphaneOtomasyonu.WPF.Helpers;
using KutuphaneOtomasyonu.WPF.Services;
using KutuphaneOtomasyonu.WPF.Commands;
using KutuphaneOtomasyonu.WPF.Models; 

namespace KutuphaneOtomasyonu.WPF.ViewModels
{
    public class UserLoginViewModel : ObservableObject
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

        public UserLoginViewModel()
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
              
                if (authResult.Role == "Uye") 
                {

                    AuthManager.SetToken(authResult.Token);

                    ErrorMessage = $"Giriş başarılı! Rol: {authResult.Role}. Yönlendiriliyorsunuz...";
                    OnLoginSuccess?.Invoke(this, authResult.Role); 
                }
                else
                {
                    ErrorMessage = "Üye girişi sadece üyeler içindir.";
                }
            }
            else
            {
                ErrorMessage = authResult.ErrorMessage;
            }
        }

       
        public event EventHandler<string> OnLoginSuccess;
    }
}