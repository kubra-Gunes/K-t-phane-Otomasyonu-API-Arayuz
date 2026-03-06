// KutuphaneOtomasyonu.WPF.Views/AdminLoginView.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using KutuphaneOtomasyonu.WPF.ViewModels;
using KutuphaneOtomasyonu.WPF.Views; // UserMainView için

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class AdminLoginView : Window
    {
        private AdminLoginViewModel _viewModel;

        public AdminLoginView()
        {
            InitializeComponent();
            _viewModel = new AdminLoginViewModel();
            this.DataContext = _viewModel; // ViewModel'ı DataContext olarak ayarla

            // ViewModel'dan gelen giriş başarılı olayını dinle
            _viewModel.OnLoginSuccess += ViewModel_OnLoginSuccess;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AdminLoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ViewModel_OnLoginSuccess(object sender, string token)
        {
            // Giriş başarılı, Yönetici Arayüzüne geçiş yap
            // AdminLoginViewModel'da zaten rol kontrolü yapıldığı için burada sadece AdminMainView'ı açıyoruz
            AdminMainView adminMainView = new AdminMainView();
            adminMainView.Show();
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