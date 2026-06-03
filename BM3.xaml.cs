using Quản_lý_công_tơ_điện.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quản_lý_công_tơ_điện
{
    public partial class BM3 : UserControl
    {
        private QuanLyCapDienContext _context;
        public BM3(QuanLyCapDienContext sharedContext)
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
                mucDichList.Insert(0, new Mucdich { MaMucDich = "", TenMucDich = "Tất cả" });
                cboMucDich.ItemsSource = mucDichList;
                cboMucDich.DisplayMemberPath = "TenMucDich";
                cboMucDich.SelectedValuePath = "MaMucDich";
                cboMucDich.SelectedIndex = 0;

                var soPhaList = _context.Loaiphas.ToList();
                soPhaList.Insert(0, new Loaipha { MaSoPha = "", TenSoPha = "Tất cả" });
                cboSoPha.ItemsSource = soPhaList;
                cboSoPha.DisplayMemberPath = "TenSoPha";
                cboSoPha.SelectedValuePath = "MaSoPha";
                cboSoPha.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu hệ thống: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTraCuu_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            try
            {
                var query = from p in _context.Phieucapdiens
                            join lp in _context.Loaiphas on p.MaSoPha equals lp.MaSoPha
                            join md in _context.Mucdiches on p.MaMucDich equals md.MaMucDich
                            select new { p, lp, md };


                if (!string.IsNullOrWhiteSpace(txtMaPhieu.Text))
                    query = query.Where(x => x.p.MaPhieu.Contains(txtMaPhieu.Text.Trim()));

                if (!string.IsNullOrWhiteSpace(txtHoTen.Text))
                    query = query.Where(x => x.p.HoTen.Contains(txtHoTen.Text.Trim()));

                if (!string.IsNullOrWhiteSpace(txtDiaChi.Text))
                    query = query.Where(x => x.p.DiaChi.Contains(txtDiaChi.Text.Trim()));

                if (!string.IsNullOrWhiteSpace(txtTrangThai.Text))
                    query = query.Where(x => x.p.TrangThai.Contains(txtTrangThai.Text.Trim()));

                if (cboSoPha.SelectedIndex > 0)
                {
                    string selectedPha = cboSoPha.SelectedValue.ToString();
                    query = query.Where(x => x.p.MaSoPha == selectedPha);
                }

                if (cboMucDich.SelectedIndex > 0)
                {
                    string selectedMucDich = cboMucDich.SelectedValue.ToString();
                    query = query.Where(x => x.p.MaMucDich == selectedMucDich);
                }

                if (dtpTuNgay.SelectedDate.HasValue)
                {
                    DateTime tuNgay = dtpTuNgay.SelectedDate.Value.Date;
                    query = query.Where(x => x.p.ThoiGianGui >= tuNgay);
                }

                if (dtpDenNgay.SelectedDate.HasValue)
                {
                    DateTime denNgay = dtpDenNgay.SelectedDate.Value.Date.AddDays(1);
                    query = query.Where(x => x.p.ThoiGianGui < denNgay);
                }

                var rawData = query.OrderByDescending(x => x.p.ThoiGianGui).ToList();

                var displayData = rawData.Select((item, index) => new
                {
                    STT = index + 1,
                    ThoiGianGui = item.p.ThoiGianGui.ToString("dd/MM/yyyy HH:mm"),
                    HoTen = item.p.HoTen,
                    DiaChi = item.p.DiaChi,
                    SoPha = item.lp.TenSoPha,
                    MucDich = item.md.TenMucDich,
                    TrangThai = item.p.TrangThai
                }).ToList();

                dgvKetQua.ItemsSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình tra cứu: " + ex.Message, "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            txtMaPhieu.Clear();
            txtHoTen.Clear();
            txtDiaChi.Clear();
            txtTrangThai.Clear();

            cboSoPha.SelectedIndex = 0;
            cboMucDich.SelectedIndex = 0;

            dtpTuNgay.SelectedDate = null;
            dtpDenNgay.SelectedDate = null;

            dgvKetQua.ItemsSource = null;
        }
    }
}
