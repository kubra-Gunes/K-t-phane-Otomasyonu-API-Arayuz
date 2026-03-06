// KutuphaneOtomasyonu.WPF.Views/UpdateBookView.xaml.cs
using System.Text.RegularExpressions; // Regex için ekleyin
using System.Windows.Controls; // TextBox için ekleyin
using System.Windows.Input; // TextCompositionEventArgs için ekleyin

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class UpdateBookView : UserControl
    {
        public UpdateBookView()
        {
            InitializeComponent();
        }

        // Sadece sayısal girişlere izin veren metod
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Sadece sayı olmayan karakterleri eşleştir
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}