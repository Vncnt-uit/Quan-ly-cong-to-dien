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
    public partial class BM1 : UserControl
    {
        private QuanLyCapDienContext _context;
        private bool _isCccdDuplicate = false;

        public BM1(QuanLyCapDienContext sharedContext)
        {

            InitializeComponent();
            _context = sharedContext;

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                var mucDichList = _context.Mucdiches.ToList();
                cboMucDich.ItemsSource = mucDichList;
                cboMucDich.DisplayMemberPath = "TenMucDich";
                cboMucDich.SelectedValuePath = "MaMucDich";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải mục đích sử dụng: " + ex.Message);
            }

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            btnHuy.IsEnabled = false;

            GenerateMaYeuCau();
            _isCccdDuplicate = false;

            CheckIfFormIsEmpty(null, null);
        }

        private void GenerateMaYeuCau()
        {
            try
            {
                var lastRecord = _context.Phieucapdiens
                                         .OrderByDescending(p => p.MaPhieu)
                                         .FirstOrDefault();

                if (lastRecord != null)
                {
                    string lastId = lastRecord.MaPhieu;
                    int nextNum = int.Parse(lastId.Substring(3)) + 1;
                    lblMaYeuCau.Text = "PCD" + nextNum.ToString("D3");
                }
                else
                {
                    lblMaYeuCau.Text = "PCD001";
                }
            }
            catch (Exception ex)
            {
                lblMaYeuCau.Text = "PCD_ERR";
                MessageBox.Show("Lỗi tự động sinh mã yêu cầu: " + ex.Message);
            }
        }

        private void CheckIfFormIsEmpty(object sender, EventArgs e)
        {
            bool isAnyBoxFilled = !string.IsNullOrWhiteSpace(txtCCCD.Text) ||
                          !string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                          !string.IsNullOrWhiteSpace(txtNamSinh.Text) ||
                          !string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                          !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                          !string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                          cboMucDich.SelectedIndex != -1 ||
                          cboSoPha.SelectedIndex != -1;

            btnHuy.IsEnabled = isAnyBoxFilled;

            bool isAllFilled = !string.IsNullOrWhiteSpace(txtCCCD.Text) &&
                       !string.IsNullOrWhiteSpace(txtHoTen.Text) &&
                       !string.IsNullOrWhiteSpace(txtNamSinh.Text) &&
                       !string.IsNullOrWhiteSpace(txtSoDienThoai.Text) &&
                       !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                       !string.IsNullOrWhiteSpace(txtDiaChi.Text) &&
                       cboMucDich.SelectedIndex != -1 &&
                       cboSoPha.SelectedIndex != -1 &&
                       !_isCccdDuplicate;

            if (btnGui != null)
            {
                btnGui.IsEnabled = isAllFilled;
                btnGui.Opacity = isAllFilled ? 1.0 : 0.9;
            }
        }

        private void txtCCCD_LostFocus(object sender, RoutedEventArgs e)
        {
            string cccd = txtCCCD.Text.Trim();

            txtCCCD.BorderBrush = new SolidColorBrush(Colors.Gray);
            lblCCCDError.Visibility = Visibility.Collapsed;
            _isCccdDuplicate = false;

            if (lblCCCDError != null)
            {
                lblCCCDError.Visibility = Visibility.Collapsed;
            }

            if (!string.IsNullOrEmpty(cccd))
            {
                bool cccdExists = _context.Phieucapdiens.Any(p => p.Cccd == cccd);

                if (cccdExists)
                {
                    txtCCCD.BorderBrush = new SolidColorBrush(Colors.Red);
                    lblCCCDError.Visibility = Visibility.Visible;
                    _isCccdDuplicate = true;
                }
            }
            CheckIfFormIsEmpty(sender, e);
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
                if (DateOnly.TryParse(txtNamSinh.Text, out DateOnly parsedDate))
                {
                    dob = parsedDate;
                }
                else if (int.TryParse(txtNamSinh.Text, out int year))
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
                    MaSoPha = cboSoPha.SelectedValue!.ToString()!,
                    TrangThai = "Chưa xử lý"
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
            cboSoPha.SelectedIndex = -1;

            txtCCCD.BorderBrush = new SolidColorBrush(Colors.Gray);
            lblCCCDError.Visibility = Visibility.Collapsed;

            if (lblCCCDError != null)
            {
                lblCCCDError.Visibility = Visibility.Collapsed;
            }

            PrepareNewForm();

        }
    }
}
