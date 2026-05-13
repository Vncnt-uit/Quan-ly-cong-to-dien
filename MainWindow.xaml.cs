using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quản_lý_công_tơ_điện.Models;

namespace Quản_lý_công_tơ_điện
{
    public partial class MainWindow : Window
    {
        // Initialize the database context
        private QuanLyCapDienContext _context = new QuanLyCapDienContext();

        public MainWindow()
        {

            InitializeComponent();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var mucDichList = _context.Mucdiches.ToList();
            cboMucDich.ItemsSource = mucDichList;
            cboMucDich.DisplayMemberPath = "TenMucDich";
            cboMucDich.SelectedValuePath = "MaMucDich";

            PrepareNewForm();
        }
        private void PrepareNewForm()
        {
            lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            int totalRecords = _context.Phieucapdiens.Count();
            string nextId = $"PCD{(totalRecords + 1).ToString("D3")}";
            lblMaYeuCau.Text = nextId;

            btnHuy.IsEnabled = false;
        }
        private void CheckIfFormIsEmpty(object sender, EventArgs e)
        {
            bool isAnyBoxFilled = !string.IsNullOrWhiteSpace(txtCCCD.Text) ||
                                  !string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                                  !string.IsNullOrWhiteSpace(txtNamSinh.Text) ||
                                  !string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                                  !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                                  !string.IsNullOrWhiteSpace(txtDiaChi.Text);

            bool isAnyComboSelected = cboMucDich.SelectedIndex != -1 ||
                                        cboSoPha.SelectedIndex != -1;

            btnHuy.IsEnabled = isAnyBoxFilled || isAnyComboSelected; ;
        }

        private void txtCCCD_LostFocus(object sender, RoutedEventArgs e)
        {
            string cccd = txtCCCD.Text.Trim();
            if (!string.IsNullOrEmpty(cccd))
            {
                bool cccdExists = _context.Phieucapdiens.Any(p => p.Cccd == cccd);

                if (cccdExists)
                {
                    MessageBox.Show("Số CCCD này đã được đăng ký trước đó! Mỗi người chỉ được đăng ký một lần.",
                                    "CCCD Đã Tồn Tại", MessageBoxButton.OK, MessageBoxImage.Warning);

                    txtCCCD.Focus();
                    txtCCCD.SelectAll();
                }
            }
        }

        private void cboMucDich_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMucDich.SelectedValue != null)
            {
                string selectedMucDich = cboMucDich.SelectedValue.ToString();

                var allowedPhases = _context.Cauhinhcapdiens
                    .Where(c => c.MaMucDich == selectedMucDich)
                    .Select(c => c.MaSoPhaNavigation)
                    .ToList();

                cboSoPha.ItemsSource = allowedPhases;
                cboSoPha.DisplayMemberPath = "TenSoPha";
                cboSoPha.SelectedValuePath = "MaSoPha";

                if (allowedPhases.Count > 0)
                    cboSoPha.SelectedIndex = 0;

                CheckIfFormIsEmpty(sender, e);
            }
        }

        private void btnGui_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCCCD.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiaChi.Text) || cboMucDich.SelectedValue == null || cboSoPha.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string currentCCCD = txtCCCD.Text.Trim();
            if (_context.Phieucapdiens.Any(p => p.Cccd == currentCCCD))
            {
                MessageBox.Show("Không thể lưu! Số CCCD này đã tồn tại trong hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string currentId = lblMaYeuCau.Text;

                DateOnly? dob = null;
                if (int.TryParse(txtNamSinh.Text, out int year))
                {
                    dob = new DateOnly(year, 1, 1);
                }

                var newPhieu = new Phieucapdien
                {
                    MaPhieu = currentId,
                    ThoiGianGui = DateTime.Now,
                    Cccd = currentCCCD,
                    HoTen = txtHoTen.Text,
                    NamSinh = dob,
                    SoDienThoai = txtSoDienThoai.Text,
                    Email = txtEmail.Text,
                    DiaChi = txtDiaChi.Text,
                    MaMucDich = cboMucDich.SelectedValue!.ToString()!,
                    MaSoPha = cboSoPha.SelectedValue!.ToString()!
                };

                _context.Phieucapdiens.Add(newPhieu);
                _context.SaveChanges();

                MessageBox.Show($"Đăng ký thành công! Mã yêu cầu của bạn là: {currentId}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtCCCD.Clear();
            txtHoTen.Clear();
            txtNamSinh.Clear();
            txtSoDienThoai.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            cboMucDich.SelectedIndex = -1;
            cboSoPha.ItemsSource = null;

            PrepareNewForm();
        }
    }
}