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
    public partial class BM2 : UserControl
    {
        private QuanLyCapDienContext _context;
        private bool _isMaCongToDuplicate = false;

        public BM2(QuanLyCapDienContext sharedContext)
        {
            InitializeComponent();
            _context = sharedContext;

            LoadInitialData();
        }

        private void LoadInitialData()
        {

            try
            {
                var processedTickets = _context.Bienbancapdiens.Select(b => b.MaPhieu).ToList();

                var pendingTickets = _context.Phieucapdiens
                                             .Where(p => !processedTickets.Contains(p.MaPhieu))
                                             .Select(p => p.MaPhieu)
                                             .ToList();

                cboMaPhieu.ItemsSource = pendingTickets;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách phiếu: " + ex.Message);
            }
            PrepareNewForm();
        }

        private void GenerateMaBienBan()
        {
            try
            {
                var lastRecord = _context.Bienbancapdiens
                                         .OrderByDescending(b => b.MaBienBan)
                                         .FirstOrDefault();

                if (lastRecord != null)
                {
                    string lastId = lastRecord.MaBienBan;
                    int nextNum = int.Parse(lastId.Substring(2)) + 1;
                    lblMaBienBan.Text = "BB" + nextNum.ToString("D3");
                }
                else
                {
                    lblMaBienBan.Text = "BB001";
                }
            }
            catch (Exception ex)
            {
                lblMaBienBan.Text = "BB_ERR";
                MessageBox.Show("Lỗi sinh mã biên bản: " + ex.Message);
            }
        }

        private void PrepareNewForm()
        {
            lblNgayLap.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblLoaiCongToHint.Visibility = Visibility.Visible;

            dtpThoiGianBatDau.DisplayDateStart = DateTime.Today;
            dtpThoiGianBatDau.SelectedDate = DateTime.Today;

            btnHuy.IsEnabled = false;

            GenerateMaBienBan();

            _isMaCongToDuplicate = false;

            btnHuy.IsEnabled = false;

            CheckIfFormIsEmpty(null, null);
        }

        private void cboMaPhieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMaPhieu.SelectedItem == null) return;

            string selectedMaPhieu = cboMaPhieu.SelectedItem.ToString();

            try
            {
                var info = (from p in _context.Phieucapdiens
                            join lp in _context.Loaiphas on p.MaSoPha equals lp.MaSoPha
                            where p.MaPhieu == selectedMaPhieu
                            select new { p, lp }).FirstOrDefault();

                if (info != null)
                {
                    txtHoTen.Text = info.p.HoTen;
                    txtDiaChi.Text = info.p.DiaChi;
                    txtSoPhaDangKy.Text = info.lp.TenSoPha;

                    var loaiCongToPhuHop = (from lct in _context.Loaicongtos
                                            join ch in _context.Cauhinhcongtos on lct.MaLoaiCongTo equals ch.MaLoaiCongTo
                                            where ch.MaSoPha == info.p.MaSoPha
                                            select lct).ToList();

                    cboLoaiCongTo.ItemsSource = loaiCongToPhuHop;
                    cboLoaiCongTo.DisplayMemberPath = "TenLoaiCongTo";
                    cboLoaiCongTo.SelectedValuePath = "MaLoaiCongTo";

                    lblLoaiCongToHint.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy thông tin phiếu: " + ex.Message);
            }
            CheckIfFormIsEmpty(null, null);
        }

        private void txtMaCongTo_LostFocus(object sender, RoutedEventArgs e)
        {
            string maCongTo = txtMaCongTo.Text.Trim();

            txtMaCongTo.BorderBrush = new SolidColorBrush(Color.FromRgb(206, 212, 218));
            if (lblMaCongToError != null) lblMaCongToError.Visibility = Visibility.Collapsed;
            _isMaCongToDuplicate = false;

            if (!string.IsNullOrEmpty(maCongTo))
            {
                bool exists = _context.Bienbancapdiens.Any(b => b.MaCongTo == maCongTo);

                if (exists)
                {
                    txtMaCongTo.BorderBrush = new SolidColorBrush(Colors.Red);
                    if (lblMaCongToError != null) lblMaCongToError.Visibility = Visibility.Visible;
                    _isMaCongToDuplicate = true;
                }
            }

            CheckIfFormIsEmpty(sender, e);
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (cboMaPhieu.SelectedItem == null || cboLoaiCongTo.SelectedValue == null ||
                string.IsNullOrWhiteSpace(txtMaCongTo.Text) || _isMaCongToDuplicate)
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc và hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string selectedMaPhieu = cboMaPhieu.SelectedItem.ToString();

                var bb = new Bienbancapdien
                {
                    MaBienBan = lblMaBienBan.Text,
                    NgayLap = DateTime.Now,
                    ThoiGianBatDauCapDien = dtpThoiGianBatDau.SelectedDate ?? DateTime.Now,
                    MaPhieu = selectedMaPhieu,
                    MaLoaiCongTo = cboLoaiCongTo.SelectedValue.ToString(),
                    MaCongTo = txtMaCongTo.Text,
                    HangSanXuat = txtHangSanXuat.Text,
                    ViTriLapDat = txtViTriLapDat.Text,
                    ChiSoBanDau = int.TryParse(txtChiSoBanDau.Text, out int cs) ? cs : 0
                };

                if (int.TryParse(txtNamSanXuat.Text, out int nam)) bb.NamSanXuat = nam;

                var phieu = _context.Phieucapdiens.FirstOrDefault(p => p.MaPhieu == selectedMaPhieu);
                if (phieu != null)
                {
                    phieu.TrangThai = "Đã xử lý";
                }

                _context.Bienbancapdiens.Add(bb);
                _context.SaveChanges();

                MessageBox.Show("Lập biên bản cấp điện thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void CheckIfFormIsEmpty(object sender, RoutedEventArgs e)
        {   
            bool isDateValid = dtpThoiGianBatDau.SelectedDate == null ||
                       dtpThoiGianBatDau.SelectedDate.Value.Date >= DateTime.Today;

            if (lblDateError != null)
            {
                lblDateError.Visibility = isDateValid ? Visibility.Collapsed : Visibility.Visible;
            }
            bool isAnyBoxFilled = cboMaPhieu.SelectedIndex != -1 ||
                          cboLoaiCongTo.SelectedIndex != -1 ||
                          !string.IsNullOrWhiteSpace(txtMaCongTo.Text) ||
                          !string.IsNullOrWhiteSpace(txtHangSanXuat.Text) ||
                          !string.IsNullOrWhiteSpace(txtNamSanXuat.Text) ||
                          !string.IsNullOrWhiteSpace(txtChiSoBanDau.Text) ||
                          !string.IsNullOrWhiteSpace(txtViTriLapDat.Text) ||
                          (dtpThoiGianBatDau.SelectedDate != null && dtpThoiGianBatDau.SelectedDate.Value.Date != DateTime.Today);

            btnHuy.IsEnabled = isAnyBoxFilled;

            bool isAllFilled = cboMaPhieu.SelectedItem != null &&
                           cboLoaiCongTo.SelectedItem != null &&
                           !string.IsNullOrWhiteSpace(txtMaCongTo.Text) &&
                           !string.IsNullOrWhiteSpace(txtHangSanXuat.Text) &&
                           !string.IsNullOrWhiteSpace(txtNamSanXuat.Text) &&
                           !string.IsNullOrWhiteSpace(txtChiSoBanDau.Text) &&
                           !string.IsNullOrWhiteSpace(txtViTriLapDat.Text) &&
                           !_isMaCongToDuplicate &&
                           isDateValid;

            btnLuu.IsEnabled = isAllFilled;
            btnLuu.Opacity = isAllFilled ? 1.0 : 0.9;
        }

        private void ClearForm()
        {
            cboMaPhieu.SelectedIndex = -1;
            cboLoaiCongTo.ItemsSource = null;

            txtHoTen.Clear();
            txtSoPhaDangKy.Clear();
            txtDiaChi.Clear();
            txtMaCongTo.Clear();
            txtHangSanXuat.Clear();
            txtNamSanXuat.Clear();
            txtChiSoBanDau.Clear();
            txtViTriLapDat.Clear();

            txtMaCongTo.BorderBrush = new SolidColorBrush(Color.FromRgb(206, 212, 218));
            if (lblMaCongToError != null)
            {
                lblMaCongToError.Visibility = Visibility.Collapsed;
            }

            PrepareNewForm();
        }
    }
}

