using Quản_lý_công_tơ_điện.Helpers;
using Quản_lý_công_tơ_điện.Models;
using Quản_lý_công_tơ_điện.UIModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    class HoaDonTienDienViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        private int _thang;
        private int _nam;
        private string _maCongTo;
        private string _hoTenKhachHang;
        private string _diaChiLapDat;
        private int? _chiSoCu;
        private int? _chiSoMoi;
        private int? _sanLuongTieuThu;
        private decimal? _tongTien;

        private bool _hasMaCongToError;
        private string _maCongToErrorMessage;

        private string _statusMessage;
        private bool _isSuccessStatus;

        private string _maPhieuGhiToSave;
        private string _maBienBanToSave;

        public int Thang { get => _thang; set { _thang = value; OnPropertyChanged(); ClearStatus(); LoadAvailableMeters(); ProcessInput(); } }
        public int Nam { get => _nam; set { _nam = value; OnPropertyChanged(); ClearStatus(); LoadAvailableMeters(); ProcessInput(); } }
        public string MaCongTo { get => _maCongTo; set { _maCongTo = value; OnPropertyChanged(); ClearStatus(); ValidateMaCongTo(); ProcessInput(); } }

        public string HoTenKhachHang { get => _hoTenKhachHang; set { _hoTenKhachHang = value; OnPropertyChanged(); } }
        public string DiaChiLapDat { get => _diaChiLapDat; set { _diaChiLapDat = value; OnPropertyChanged(); } }
        public int? ChiSoCu { get => _chiSoCu; set { _chiSoCu = value; OnPropertyChanged(); } }
        public int? ChiSoMoi { get => _chiSoMoi; set { _chiSoMoi = value; OnPropertyChanged(); } }
        public int? SanLuongTieuThu { get => _sanLuongTieuThu; set { _sanLuongTieuThu = value; OnPropertyChanged(); } }
        public decimal? TongTien
        {
            get => _tongTien;
            set { _tongTien = value; OnPropertyChanged(); OnPropertyChanged(nameof(TongTienText)); }
        }

        public bool HasMaCongToError { get => _hasMaCongToError; set { _hasMaCongToError = value; OnPropertyChanged(); } }
        public string MaCongToErrorMessage { get => _maCongToErrorMessage; set { _maCongToErrorMessage = value; OnPropertyChanged(); } }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }
        public string TongTienText => TongTien.HasValue ? $"{TongTien:N0} VNĐ" : string.Empty;
        private ObservableCollection<string> _danhSachMaCongTo;

        public ObservableCollection<int> DanhSachThang { get; set; }
        public ObservableCollection<int> DanhSachNam { get; set; }
        public ObservableCollection<ChiTietHoaDonRow> DanhSachChiTiet { get; set; }
        public ObservableCollection<string> DanhSachMaCongTo
        {
            get => _danhSachMaCongTo;
            set { _danhSachMaCongTo = value; OnPropertyChanged(); }
        }

        public ICommand LuuCommand { get; }
        public ICommand HuyCommand { get; }

        private void ValidateMaCongTo()
        {
            if (string.IsNullOrWhiteSpace(MaCongTo))
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Vui lòng nhập mã công tơ.";
                return;
            }
            string maCongToTrimmed = MaCongTo.Trim().ToUpper();

            if (DanhSachMaCongTo == null || !DanhSachMaCongTo.Contains(maCongToTrimmed))
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Không tìm thấy mã công tơ hoặc đã lập hóa đơn trong kỳ này.";
                return;
            }

            HasMaCongToError = false;
            MaCongToErrorMessage = string.Empty;
        }

        public HoaDonTienDienViewModel(QuanLyCapDienContext context)
        {
            _db = context;

            DanhSachChiTiet = new ObservableCollection<ChiTietHoaDonRow>();
            DanhSachThang = new ObservableCollection<int>(Enumerable.Range(1, 12));
            DanhSachNam = new ObservableCollection<int>(Enumerable.Range(DateTime.Now.Year - 5, 6));

            LuuCommand = new RelayCommand(ExecuteLuu, CanExecuteLuu);
            HuyCommand = new RelayCommand(ExecuteHuy, CanExecuteHuy);

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            Thang = DateTime.Now.Month;
            Nam = DateTime.Now.Year;

            _maCongTo = string.Empty;
            OnPropertyChanged(nameof(MaCongTo));

            HasMaCongToError = false;
            MaCongToErrorMessage = string.Empty;
            ClearStatus();

            ClearAutoFillData();
        }
        private void LoadAvailableMeters()
        {
            string kyGhi = $"{Thang:D2}/{Nam}";

            var availableMeters = (from c in _db.Chitietghidiens
                                   join p in _db.Phieughidiens on c.MaPhieuGhi equals p.MaPhieuGhi
                                   join b in _db.Bienbancapdiens on c.MaBienBan equals b.MaBienBan
                                   where p.KyGhiChiSo == kyGhi
                                   && !_db.Hoadontiendiens.Any(h => h.MaPhieuGhi == c.MaPhieuGhi && h.MaBienBan == c.MaBienBan)
                                   select b.MaCongTo).Distinct().ToList();

            DanhSachMaCongTo = new ObservableCollection<string>(availableMeters);
        }

        private void ClearStatus()
        {
            if (!string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = string.Empty;
                IsSuccessStatus = true;
            }
        }

        private void ClearAutoFillData()
        {
            HoTenKhachHang = string.Empty;
            DiaChiLapDat = string.Empty;
            ChiSoCu = null;
            ChiSoMoi = null;
            SanLuongTieuThu = null;
            TongTien = null;
            DanhSachChiTiet.Clear();
            _maPhieuGhiToSave = null;
            _maBienBanToSave = null;
        }

        private void ProcessInput()
        {
            ClearAutoFillData();

            if (HasMaCongToError || string.IsNullOrWhiteSpace(MaCongTo)) return;

            string maCongToTrimmed = MaCongTo.Trim().ToUpper();
            string kyGhi = $"{Thang:D2}/{Nam}";

            var readingData = (from c in _db.Chitietghidiens
                               join p in _db.Phieughidiens on c.MaPhieuGhi equals p.MaPhieuGhi
                               join b in _db.Bienbancapdiens on c.MaBienBan equals b.MaBienBan
                               join kh in _db.Phieucapdiens on b.MaPhieu equals kh.MaPhieu
                               where p.KyGhiChiSo == kyGhi && b.MaCongTo == maCongToTrimmed
                               select new
                               {
                                   c.MaPhieuGhi,
                                   c.MaBienBan,
                                   kh.HoTen,
                                   DiaChi = b.ViTriLapDat ?? kh.DiaChi,
                                   c.ChiSoCu,
                                   c.ChiSoMoi,
                                   c.SanLuongTieuThu
                               }).FirstOrDefault();

            if (readingData != null)
            {
                _maPhieuGhiToSave = readingData.MaPhieuGhi;
                _maBienBanToSave = readingData.MaBienBan;
                HoTenKhachHang = readingData.HoTen;
                DiaChiLapDat = readingData.DiaChi;
                ChiSoCu = readingData.ChiSoCu;
                ChiSoMoi = readingData.ChiSoMoi;
                SanLuongTieuThu = readingData.SanLuongTieuThu;

                CalculateTierMath(readingData.SanLuongTieuThu);
            }
        }

        private void CalculateTierMath(int sanLuong)
        {
            decimal total = 0;
            int remaining = sanLuong;

            var rulesTable = _db.QuyDinhGiaDiens.OrderBy(q => q.Bac).ToList();

            foreach (var rule in rulesTable)
            {
                if (remaining <= 0) break;

                int usedInThisTier = 0;
                string dinhMucText = "";
                int? dinhMucDbValue = null;

                if (rule.DinhMuc.HasValue)
                {
                    usedInThisTier = Math.Min(remaining, rule.DinhMuc.Value);
                    dinhMucText = rule.DinhMuc.Value.ToString();
                    dinhMucDbValue = rule.DinhMuc.Value;
                }
                else
                {
                    usedInThisTier = remaining;
                    dinhMucText = "Còn lại";
                    dinhMucDbValue = null;
                }

                decimal cost = usedInThisTier * rule.DonGia;
                total += cost;
                remaining -= usedInThisTier;

                DanhSachChiTiet.Add(new ChiTietHoaDonRow
                {
                    BacSo = rule.Bac,
                    DonGia = rule.DonGia,
                    DinhMucThucTeText = dinhMucText,
                    DinhMucDb = dinhMucDbValue,
                    DienNangTieuThu = usedInThisTier,
                    ThanhTien = cost
                });
            }

            TongTien = total;
        }

        private bool CanExecuteHuy(object obj)
        {
            return Thang != DateTime.Now.Month ||
                   Nam != DateTime.Now.Year ||
                   !string.IsNullOrWhiteSpace(MaCongTo);
        }

        private bool CanExecuteLuu(object obj)
        {
            return !string.IsNullOrWhiteSpace(MaCongTo) &&
                   !HasMaCongToError &&
                   string.IsNullOrEmpty(StatusMessage) &&
                   DanhSachChiTiet.Any() &&
                   TongTien.HasValue;
        }

        private void ExecuteHuy(object obj)
        {
            PrepareNewForm();
            ClearStatus();
        }

        private void ExecuteLuu(object obj)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    string newMaHoaDon = "HD" + DateTime.Now.ToString("yyMMddHHmmss");

                    var hoadon = new Hoadontiendien
                    {
                        MaHoaDon = newMaHoaDon,
                        TongTien = TongTien.Value,
                        MaPhieuGhi = _maPhieuGhiToSave,
                        MaBienBan = _maBienBanToSave
                    };
                    _db.Hoadontiendiens.Add(hoadon);

                    foreach (var row in DanhSachChiTiet)
                    {
                        var chiTietDb = new Chitiethoadon
                        {
                            MaHoaDon = newMaHoaDon,
                            Bac = row.BacSo,
                            DonGia = row.DonGia,
                            DinhMuc = row.DinhMucDb,
                            DienNangTieuThu = row.DienNangTieuThu,
                            ThanhTien = row.ThanhTien
                        };
                        _db.Chitiethoadons.Add(chiTietDb);
                    }

                    _db.SaveChanges();
                    transaction.Commit();

                    PrepareNewForm();
                    IsSuccessStatus = true;
                    StatusMessage = $"LẬP HÓA ĐƠN {newMaHoaDon} THÀNH CÔNG!";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    IsSuccessStatus = false;
                    StatusMessage = "CÓ LỖI XẢY RA KHI LẬP HÓA ĐƠN: " + ex.Message;
                }
            }
        }
        public override void Refresh()
        {
            try
            {
                LoadAvailableMeters();

                if (!string.IsNullOrWhiteSpace(MaCongTo))
                {
                    ValidateMaCongTo();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải lại BM5: {ex.Message}");
            }
        }
    }
}