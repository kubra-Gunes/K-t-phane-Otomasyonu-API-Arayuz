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
    public partial class CategoryListView : UserControl
    {
        public CategoryListView()
        {
            InitializeComponent();
            this.Loaded += CategoryListView_Loaded;
        }

        private async void CategoryListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CategoryListViewModel viewModel)
            {
                await viewModel.LoadCategories();
            }
        }
    }
}