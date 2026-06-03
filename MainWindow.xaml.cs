using System.Windows;
using Quản_lý_công_tơ_điện.Models;

namespace Quản_lý_công_tơ_điện
{
    public partial class MainWindow : Window
    {

        private QuanLyCapDienContext _context;

        public MainWindow(QuanLyCapDienContext sharedContext)
        {
            InitializeComponent();
            _context = sharedContext;

            MainContent.Content = new BM1(_context);
        }

        private void btnBM1_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BM1(_context);
        }

        private void btnBM2_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BM2(_context);
        }

        private void btnBM3_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BM3(_context);
        }
    }
}