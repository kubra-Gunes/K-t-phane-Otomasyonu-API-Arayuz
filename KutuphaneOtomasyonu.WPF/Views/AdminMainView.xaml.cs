using System.Windows;
using System.Windows.Input; // KeyEventArgs için
using KutuphaneOtomasyonu.WPF.ViewModels;

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class AdminMainView : Window
    {
        public AdminMainView()
        {
            InitializeComponent();
            this.DataContext = new AdminMainViewModel();
        }

        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (DataContext is AdminMainViewModel viewModel)
        //        {
        //            viewModel.SearchCommand.Execute(null);
        //        }
        //    }
        //}
    }
}