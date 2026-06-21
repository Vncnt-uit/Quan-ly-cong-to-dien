using Quản_lý_công_tơ_điện.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.Views
{
    public partial class DanhMuc : Window
    {
        public DanhMuc(DanhMucViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainLayout.Focus();
        }
    }
}