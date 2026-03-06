// KutuphaneOtomasyonu.WPF.Views/BorrowBookView.xaml.cs
using System.Windows.Controls;

namespace KutuphaneOtomasyonu.WPF.Views
{
    /// <summary>
    /// BorrowBookView.xaml için etkileşim mantığı
    /// </summary>
    public partial class BorrowBookView : UserControl
    {
        public BorrowBookView()
        {
            InitializeComponent();
            // DataContext, AdminMainView.xaml'deki DataTemplate tarafından atanacağı için burada set etmiyoruz.
        }
    }
}