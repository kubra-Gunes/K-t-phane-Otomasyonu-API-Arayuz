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
using KutuphaneOtomasyonu.WPF.Views;



using global::KutuphaneOtomasyonu.WPF.ViewModels;

namespace KutuphaneOtomasyonu.WPF.Views
{
    public partial class MyCommentsView : UserControl
    {
        public MyCommentsView()
        {
            InitializeComponent();
        }

        public MyCommentsView(MyCommentsViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}