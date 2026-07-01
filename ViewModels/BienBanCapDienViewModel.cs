using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class BienBanCapDienViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        public Action RequestGoHome { get; set; }

        private string _maBienBan;
        private string _ngayLap;
        private DateTime? _thoiGianBatDau;

        private string _selectedMaPhieu;
        private string _hoTen;
        private string _diaChi;
        private string _soPhaDangKy;
        private string _maCongTo;
        private string _hangSanXuat;
        private string _namSanXuat;
        private string _chiSoBanDau;
        private string _viTriLapDat;

        private string _tenLoaiCongTo;
        private string _maLoaiCongToDB;

        private bool _hasMaPhieuError;
        private string _maPhieuErrorMessage;

        private bool _hasDateError;
        private string _dateErrorMessage;

        private bool _hasMaCongToError;
        private string _maCongToErrorMessage;

        private bool _hasHangSanXuatError;
        private string _hangSanXuatErrorMessage;

        private bool _hasNamSanXuatError;
        private string _namSanXuatErrorMessage;

        private bool _hasChiSoError;
        private string _chiSoErrorMessage;

        private bool _hasViTriError;
        private string _viTriErrorMessage;

        private string _statusMessage;
        private bool _isSuccessStatus;

        private List<string> _maPhieuList;

        public DateTime NgayHienTai { get; set; } = DateTime.Today;
        public string MaBienBan { get => _maBienBan; set { _maBienBan = value; OnPropertyChanged(); } }
        public string NgayLap { get => _ngayLap; set { _ngayLap = value; OnPropertyChanged(); } }
        public string HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); } }
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }
        public string SoPhaDangKy { get => _soPhaDangKy; set { _soPhaDangKy = value; OnPropertyChanged(); } }
        public string TenLoaiCongTo { get => _tenLoaiCongTo; set { _tenLoaiCongTo = value; OnPropertyChanged(); } }

        public string SelectedMaPhieu { get => _selectedMaPhieu; set { _selectedMaPhieu = value; OnPropertyChanged(); ClearStatus(); ValidateMaPhieu(); LoadPhieuDetails(); } }
        public DateTime? ThoiGianBatDau { get => _thoiGianBatDau; set { _thoiGianBatDau = value; OnPropertyChanged(); ClearStatus(); ValidateThoiGianBatDau(); } }
        public string MaCongTo { get => _maCongTo; set { _maCongTo = value; OnPropertyChanged(); ClearStatus(); ValidateMaCongTo(); } }
        public string HangSanXuat { get => _hangSanXuat; set { _hangSanXuat = value; OnPropertyChanged(); ClearStatus(); ValidateHangSanXuat(); } }
        public string NamSanXuat { get => _namSanXuat; set { _namSanXuat = value; OnPropertyChanged(); ClearStatus(); ValidateNamSanXuat(); } }
        public string ChiSoBanDau { get => _chiSoBanDau; set { _chiSoBanDau = value; OnPropertyChanged(); ClearStatus(); ValidateChiSoBanDau(); } }
        public string ViTriLapDat { get => _viTriLapDat; set { _viTriLapDat = value; OnPropertyChanged(); ClearStatus(); ValidateViTriLapDat(); } }

        public bool HasMaPhieuError { get => _hasMaPhieuError; set { _hasMaPhieuError = value; OnPropertyChanged(); } }
        public string MaPhieuErrorMessage { get => _maPhieuErrorMessage; set { _maPhieuErrorMessage = value; OnPropertyChanged(); } }

        public bool HasDateError { get => _hasDateError; set { _hasDateError = value; OnPropertyChanged(); } }
        public string DateErrorMessage { get => _dateErrorMessage; set { _dateErrorMessage = value; OnPropertyChanged(); } }

        public bool HasMaCongToError { get => _hasMaCongToError; set { _hasMaCongToError = value; OnPropertyChanged(); } }
        public string MaCongToErrorMessage { get => _maCongToErrorMessage; set { _maCongToErrorMessage = value; OnPropertyChanged(); } }

        public bool HasHangSanXuatError { get => _hasHangSanXuatError; set { _hasHangSanXuatError = value; OnPropertyChanged(); } }
        public string HangSanXuatErrorMessage { get => _hangSanXuatErrorMessage; set { _hangSanXuatErrorMessage = value; OnPropertyChanged(); } }

        public bool HasNamSanXuatError { get => _hasNamSanXuatError; set { _hasNamSanXuatError = value; OnPropertyChanged(); } }
        public string NamSanXuatErrorMessage { get => _namSanXuatErrorMessage; set { _namSanXuatErrorMessage = value; OnPropertyChanged(); } }

        public bool HasChiSoError { get => _hasChiSoError; set { _hasChiSoError = value; OnPropertyChanged(); } }
        public string ChiSoErrorMessage { get => _chiSoErrorMessage; set { _chiSoErrorMessage = value; OnPropertyChanged(); } }

        public bool HasViTriError { get => _hasViTriError; set { _hasViTriError = value; OnPropertyChanged(); } }
        public string ViTriErrorMessage { get => _viTriErrorMessage; set { _viTriErrorMessage = value; OnPropertyChanged(); } }


        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }
        
        public List<string> MaPhieuList { get => _maPhieuList; set { _maPhieuList = value; OnPropertyChanged(); } }

        public ICommand LuuCommand { get; }
        public ICommand ThoatCommand { get; }

        public BienBanCapDienViewModel(QuanLyCapDienContext context)
        {
            _db = context;

            LuuCommand = new RelayCommand(ExecuteLuu);
            ThoatCommand = new RelayCommand(ExecuteThoat);

            LoadInitialData();
        }
        private void ValidateMaPhieu()
        {
            if (string.IsNullOrWhiteSpace(SelectedMaPhieu))
            {
                HasMaPhieuError = true;
                MaPhieuErrorMessage = "* Vui lòng chọn hoặc nhập mã phiếu.";
                return;
            }
            if (MaPhieuList == null || !MaPhieuList.Contains(SelectedMaPhieu.Trim()))
            {
                HasMaPhieuError = true;
                MaPhieuErrorMessage = "* Mã phiếu không tồn tại hoặc đã được xử lý.";
                return;
            }
            HasMaPhieuError = false;
            MaPhieuErrorMessage = string.Empty;
        }

        private void ValidateThoiGianBatDau()
        {
            if (ThoiGianBatDau.HasValue && ThoiGianBatDau.Value.Date < DateTime.Today)
            {
                HasDateError = true;
                DateErrorMessage = "* Ngày cấp điện không được trước ngày lập phiếu!";
                return;
            }
            HasDateError = false;
            DateErrorMessage = string.Empty;
        }

        private void ValidateMaCongTo()
        {
            if (string.IsNullOrWhiteSpace(MaCongTo))
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Vui lòng nhập mã công tơ.";
                return;
            }
            if (_db.Bienbancapdiens.Any(b => b.MaCongTo == MaCongTo.Trim()))
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Mã công tơ này đã được sử dụng!";
                return;
            }
            if(!System.Text.RegularExpressions.Regex.IsMatch(MaCongTo.Trim(), @"^[A-Za-z0-9]+$"))
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Mã công tơ không hợp lệ.";
                return;
            }
            if(MaCongTo.Trim().Length > 50)
            {
                HasMaCongToError = true;
                MaCongToErrorMessage = "* Mã công tơ tối đa 50 kí tự.";
                return;
            }
            HasMaCongToError = false;
            MaCongToErrorMessage = string.Empty;
        }

        private void ValidateHangSanXuat()
        {
            if (string.IsNullOrWhiteSpace(HangSanXuat))
            {
                HasHangSanXuatError = true;
                HangSanXuatErrorMessage = "* Vui lòng nhập hãng sản xuất.";
                return;
            }
            if(HangSanXuat.Trim().Length > 100)
            {
                HasHangSanXuatError = true;
                HangSanXuatErrorMessage = "* Hãng sản xuất tối đa 100 kí tự.";
                return;
            }
            HasHangSanXuatError = false;
            HangSanXuatErrorMessage = string.Empty;
        }

        private void ValidateNamSanXuat()
        {
            if (string.IsNullOrWhiteSpace(NamSanXuat))
            {
                HasNamSanXuatError = true;
                NamSanXuatErrorMessage = "* Vui lòng nhập năm sản xuất.";
                return;
            }
            if (!int.TryParse(NamSanXuat, out int nam) || nam < 1900 || nam > DateTime.Now.Year)
            {
                HasNamSanXuatError = true;
                NamSanXuatErrorMessage = "* Năm sản xuất không hợp lệ. (Từ 1900 đến " + DateTime.Now.Year + ")";
                return;
            }
            HasNamSanXuatError = false;
            NamSanXuatErrorMessage = string.Empty;
        }

        private void ValidateChiSoBanDau()
        {
            if (string.IsNullOrWhiteSpace(ChiSoBanDau))
            {
                HasChiSoError = true;
                ChiSoErrorMessage = "* Vui lòng nhập chỉ số ban đầu.";
                return;
            }
            if (!int.TryParse(ChiSoBanDau, out int chiSo) || chiSo < 0)
            {
                HasChiSoError = true;
                ChiSoErrorMessage = "* Chỉ số không hợp lệ hoặc quá lớn.";
                return;
            }
            else { HasChiSoError = false; ChiSoErrorMessage = string.Empty; }
        }

        private void ValidateViTriLapDat()
        {
            if (string.IsNullOrWhiteSpace(ViTriLapDat))
            {
                HasViTriError = true;
                ViTriErrorMessage = "* Vui lòng nhập vị trí lắp đặt.";
                return;
            }
            if(ViTriLapDat.Trim().Length > 255)
            {
                HasViTriError = true;
                ViTriErrorMessage = "* Vị trí lắp đặt tối đa 255 kí tự.";
                return;
            }
            HasViTriError = false;
            ViTriErrorMessage = string.Empty;
        }
        private void ClearStatus()
        {
            if (!string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = string.Empty;
            }
        }
        private void LoadInitialData()
        {
            try
            {
                var processed = _db.Bienbancapdiens.Select(b => b.MaPhieu).ToList();
                MaPhieuList = _db.Phieucapdiens.Where(p => !processed.Contains(p.MaPhieu)).Select(p => p.MaPhieu).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi kết nối CSDL: Không tải được danh sách phiếu.";
                IsSuccessStatus = false;
                System.Diagnostics.Debug.WriteLine($"Lỗi tải dữ liệu: {ex.Message}");
            }

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            NgayLap = DateTime.Now.ToString("dd/MM/yyyy");
            ThoiGianBatDau = DateTime.Today;

            ChiSoBanDau = "0";
            GenerateMaBienBan();

            HasMaPhieuError = false;
            HasDateError = false;
            HasMaCongToError = false;
            HasHangSanXuatError = false;
            HasNamSanXuatError = false;
            HasChiSoError = false;
            HasViTriError = false;

            MaPhieuErrorMessage = string.Empty;
            DateErrorMessage = string.Empty;
            MaCongToErrorMessage = string.Empty;
            HangSanXuatErrorMessage = string.Empty;
            NamSanXuatErrorMessage = string.Empty;
            ChiSoErrorMessage = string.Empty;
            ViTriErrorMessage = string.Empty;
        }

        private void GenerateMaBienBan()
        {
            try
            {
                var lastRecord = _db.Bienbancapdiens
                                    .OrderByDescending(b => b.MaBienBan.Length)
                                    .ThenByDescending(b => b.MaBienBan)
                                    .FirstOrDefault();

                if (lastRecord != null)
                {
                    string lastId = lastRecord.MaBienBan;
                    int nextNum = int.Parse(lastId.Substring(2)) + 1;
                    MaBienBan = "BB" + nextNum.ToString("D3");
                }
                else
                {
                    MaBienBan = "BB001";
                }
            }
            catch
            {
                MaBienBan = "BB_ERR";
            }
        }

        private void LoadPhieuDetails()
        {
            HoTen = string.Empty;
            DiaChi = string.Empty;
            SoPhaDangKy = string.Empty;
            TenLoaiCongTo = string.Empty;
            _maLoaiCongToDB = string.Empty;
            if (HasMaPhieuError || string.IsNullOrEmpty(SelectedMaPhieu))
            {
                HoTen = DiaChi = SoPhaDangKy = TenLoaiCongTo = _maLoaiCongToDB = string.Empty;
                return;
            }

            var info = (from p in _db.Phieucapdiens
                        join lp in _db.Loaiphas on p.MaSoPha equals lp.MaSoPha
                        where p.MaPhieu == SelectedMaPhieu
                        select new { p, lp }).FirstOrDefault();

            if (info != null)
            {
                HoTen = info.p.HoTen;
                DiaChi = info.p.DiaChi;
                SoPhaDangKy = info.lp.TenSoPha;

                var loaiCongTo = _db.Cauhinhcongtos
                    .Where(c => c.MaSoPha == info.p.MaSoPha)
                    .Select(c => c.MaLoaiCongToNavigation)
                    .FirstOrDefault();

                if (loaiCongTo != null)
                {
                    TenLoaiCongTo = loaiCongTo.TenLoaiCongTo;
                    _maLoaiCongToDB = loaiCongTo.MaLoaiCongTo;
                }
            }
        }

        private void ResetFormFields()
        {
            SelectedMaPhieu = string.Empty;
            MaCongTo = string.Empty;
            HangSanXuat = string.Empty;
            NamSanXuat = string.Empty;
            ChiSoBanDau = string.Empty;
            ViTriLapDat = string.Empty;
            HoTen = string.Empty;
            DiaChi = string.Empty;
            SoPhaDangKy = string.Empty;
            TenLoaiCongTo = string.Empty;
            _maLoaiCongToDB = string.Empty;

            StatusMessage = string.Empty;
            IsSuccessStatus = true;

            PrepareNewForm();
        }

        private void ExecuteThoat(object obj)
        {
            ResetFormFields();
            RequestGoHome?.Invoke();
        }

        private void ExecuteLuu(object obj)
        {
            ValidateMaPhieu();
            ValidateThoiGianBatDau();
            ValidateMaCongTo();
            ValidateHangSanXuat();
            ValidateNamSanXuat();
            ValidateChiSoBanDau();
            ValidateViTriLapDat();

            if (HasMaPhieuError || HasDateError || HasMaCongToError || HasHangSanXuatError ||
                HasNamSanXuatError || HasChiSoError || HasViTriError)
            {
                IsSuccessStatus = false;
                StatusMessage = "Vui lòng kiểm tra lại các thông tin chưa hợp lệ.";
                return;
            }
            try
            {
                var newBienBan = new Bienbancapdien
                {
                    MaBienBan = MaBienBan,
                    NgayLap = DateTime.Now,
                    ThoiGianBatDauCapDien = ThoiGianBatDau,
                    MaPhieu = SelectedMaPhieu,
                    MaLoaiCongTo = _maLoaiCongToDB,
                    MaCongTo = MaCongTo,
                    HangSanXuat = HangSanXuat,
                    NamSanXuat = int.Parse(NamSanXuat),
                    ChiSoBanDau = int.Parse(ChiSoBanDau),
                    ViTriLapDat = ViTriLapDat
                };

                var phieu = _db.Phieucapdiens.FirstOrDefault(p => p.MaPhieu == SelectedMaPhieu);
                if (phieu != null) phieu.TrangThai = "Đã xử lý";

                _db.Bienbancapdiens.Add(newBienBan);
                _db.SaveChanges();
                
                LoadInitialData();

                ResetFormFields();

                IsSuccessStatus = true;
                StatusMessage = $"LẬP BIÊN BẢN THÀNH CÔNG!";
            }
            catch (Exception ex)
            {
                StatusMessage = "CÓ LỖI XẢY RA KHI LẬP BIÊN BẢN.";
                IsSuccessStatus = false;

                System.Diagnostics.Debug.WriteLine($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }
        public override void Refresh()
        {
            try
            {
                var processed = _db.Bienbancapdiens.Select(b => b.MaPhieu).ToList();
                MaPhieuList = _db.Phieucapdiens.Where(p => !processed.Contains(p.MaPhieu)).Select(p => p.MaPhieu).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải lại Biên bản cấp điện: {ex.Message}");
            }
        }
    }
}