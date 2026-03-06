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


namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class AuthorListView : UserControl
    {
        public AuthorListView()
        {
            InitializeComponent();
            //this.Loaded += AuthorListView_Loaded;
        }

        //private async void AuthorListView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is AuthorListViewModel viewModel)
        //    {
        //        await viewModel.LoadAuthors();
        //    }
        //}
    }
}