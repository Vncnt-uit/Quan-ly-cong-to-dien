using Quản_lý_công_tơ_điện.Models;
using Microsoft.EntityFrameworkCore; // <-- You need this for UseSqlServer
using System.Configuration;
using System.Data;
using System.Windows;

namespace Quản_lý_công_tơ_điện
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Prepare the setup options for Entity Framework
            var optionsBuilder = new DbContextOptionsBuilder<QuanLyCapDienContext>();
            
            // Note: If you don't have an App.config file yet, you can replace the ConfigurationManager line 
            // with your raw string: "Data Source=VINCENT;Initial Catalog=QuanLyCapDien;Integrated Security=True;TrustServerCertificate=True;"
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
            optionsBuilder.UseSqlServer(connectionString);

            // 2. Pass those options into the constructor! (This makes the red error vanish)
            var sharedContext = new QuanLyCapDienContext(optionsBuilder.Options);

            // 3. Hand it directly to MainWindow
            var mainWindow = new MainWindow(sharedContext);

            // 4. Show the window
            mainWindow.Show();
        }
    }
}
