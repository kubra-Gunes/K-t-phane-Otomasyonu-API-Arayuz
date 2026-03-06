using System.Windows.Controls;
using System.Windows;
using KutuphaneOtomasyonu.WPF.ViewModels;

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class AddBookView : UserControl
    {
        public AddBookView()
        {
            InitializeComponent();
            this.Loaded += AddBookView_Loaded;
        }

        private async void AddBookView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddBookViewModel viewModel)
            {
                viewModel.LoadRelatedDataCommand.Execute(null);

            }
        }
    }
}