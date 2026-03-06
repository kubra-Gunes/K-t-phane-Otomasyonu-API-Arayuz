using System;
using System.Windows;
using System.Windows.Controls;
using KutuphaneOtomasyonu.WPF.ViewModels;
using KutuphaneOtomasyonu.WPF.Views; // MainWindow için gerekli

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class UserLoginView : Window
    {
        private UserLoginViewModel _viewModel;

        public UserLoginView()
        {
            InitializeComponent();
            _viewModel = new UserLoginViewModel();
            this.DataContext = _viewModel;

            _viewModel.OnLoginSuccess += ViewModel_OnLoginSuccess;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UserLoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ViewModel_OnLoginSuccess(object sender, string role)
        {
            // Giriş başarılı, role göre ana pencereyi aç
            if (role == "Admin") // API'den dönen role göre kontrol
            {
                // Eğer AdminMainView diye bir pencereniz varsa onu açın
                AdminMainView adminMainView = new AdminMainView();
                adminMainView.Show();
            }
            else if (role == "Uye") // Veya diğer kullanıcı rolleri
            {
                UserMainView userMainView = new UserMainView();
                userMainView.Show();
            }
            else
            {
                // Tanımsız rol durumu
                MessageBox.Show($"Bilinmeyen rol: {role}. Lütfen sistem yöneticisi ile iletişime geçin.", "Giriş Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Pencereyi kapatma
            }

            this.Close(); // Mevcut giriş penceresini kapat
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // Ana pencereyi oluştur
            mainWindow.Show(); // Ana pencereyi göster
            this.Close(); // Mevcut Kullanıcı Giriş penceresini kapat
        }
    }
}