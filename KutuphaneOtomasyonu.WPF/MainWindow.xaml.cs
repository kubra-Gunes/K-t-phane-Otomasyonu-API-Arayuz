using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KutuphaneOtomasyonu.WPF.Views; // AdminLoginView ve UserLoginView için

namespace KutuphaneOtomasyonu.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UserLogin_Click(object sender, RoutedEventArgs e)
        {
            UserLoginView userLoginView = new UserLoginView(); // Kullanıcı Giriş Ekranını aç
            userLoginView.Show();
            this.Close(); // Ana pencereyi kapat
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginView adminLoginView = new AdminLoginView(); // Yönetici Giriş Ekranını aç
            adminLoginView.Show();
            this.Close(); // Ana pencereyi kapat
        }
    }
}