using System.Windows;
using Quản_lý_công_tơ_điện.Models;
using Quản_lý_công_tơ_điện.ViewModels;
using Quản_lý_công_tơ_điện.Views;

namespace Quản_lý_công_tơ_điện
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbContext = new QuanLyCapDienContext();

            dbContext.Database.EnsureCreated();

            var mainViewModel = new DanhMucViewModel(dbContext);
            var mainWindow = new DanhMuc(mainViewModel);

            mainWindow.Show();
        }
    }
}