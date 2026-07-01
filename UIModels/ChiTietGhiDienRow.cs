using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.UIModels
{
    public class ChiTietGhiDienRow : ObservableObject
    {
        private readonly QuanLyCapDienContext _db;
        public Action RequestValidation { get; set; }
        public Func<string> GetKyGhiChiSo { get; set; }

        private int _stt;
        private string _maCongTo;
        private string _tenKhachHang;
        private string _diaChi;
        private int? _chiSoCu;
        private int? _chiSoMoi;
        private int? _sanLuongTieuThu;
        private string _errorMessage;
        private bool _isBlockedByParent;
        private string _chiSoMoiText;

        public string MaBienBan { get; set; }
        public bool IsValidRow => !string.IsNullOrEmpty(MaCongTo) && ChiSoMoi.HasValue && SanLuongTieuThu.HasValue && string.IsNullOrEmpty(ErrorMessage);
        public int STT { get => _stt; set { _stt = value; OnPropertyChanged(); } }
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }
        public int? ChiSoCu { get => _chiSoCu; set { _chiSoCu = value; OnPropertyChanged(); } }
        public int? SanLuongTieuThu { get => _sanLuongTieuThu; set { _sanLuongTieuThu = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }
        public bool CanEnterChiSoMoi => !string.IsNullOrWhiteSpace(TenKhachHang) && !IsBlockedByParent;
        public string ChiSoMoiText
        {
            get => _chiSoMoiText;
            set
            {
                _chiSoMoiText = value;
                OnPropertyChanged();

                if (int.TryParse(_chiSoMoiText, out int result))
                {
                    ChiSoMoi = result;
                }
                else
                {
                    ChiSoMoi = null;
                }

                CalculateSanLuong();
                RequestValidation?.Invoke();
            }
        }
        public bool IsBlockedByParent
        {
            get => _isBlockedByParent;
            set
            {
                _isBlockedByParent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEnterChiSoMoi));
            }
        }
        public string TenKhachHang
        {
            get => _tenKhachHang;
            set
            {
                _tenKhachHang = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(CanEnterChiSoMoi));
            }
        }

        public string MaCongTo
        {
            get => _maCongTo;
            set
            {
                if (_maCongTo != value)
                {
                    _maCongTo = value;
                    OnPropertyChanged();

                    ClearAutoFill();

                    ChiSoMoiText = string.Empty;
                    ChiSoMoi = null;

                    if (!string.IsNullOrWhiteSpace(_maCongTo))
                    {
                        TriggerAutoFill();
                    }
                    RequestValidation?.Invoke();
                }
            }
        }

        public int? ChiSoMoi
        {
            get => _chiSoMoi;
            set
            {
                _chiSoMoi = value;
                OnPropertyChanged();
                
                CalculateSanLuong();

                RequestValidation?.Invoke();
            }
        }

        public ChiTietGhiDienRow(QuanLyCapDienContext db, int stt)
        {
            _db = db;
            STT = stt;
        }
        public void ClearAutoFill()
        {
            MaBienBan = null;
            TenKhachHang = string.Empty;
            DiaChi = string.Empty;
            ChiSoCu = null;
            SanLuongTieuThu = null;
            ErrorMessage = string.Empty;
        }
        public void TriggerAutoFill() => AutoFillKhachHang();

        private void AutoFillKhachHang()
        {
            if (string.IsNullOrWhiteSpace(MaCongTo)) return;

            var info = (from b in _db.Bienbancapdiens
                        join p in _db.Phieucapdiens on b.MaPhieu equals p.MaPhieu
                        where b.MaCongTo == MaCongTo.Trim()
                        select new { b.MaBienBan, b.ChiSoBanDau, p.HoTen, p.DiaChi }).FirstOrDefault();

            if (info != null)
            {
                string currentKyGhi = GetKyGhiChiSo?.Invoke();
                bool isAlreadyRecorded = (from c in _db.Chitietghidiens
                                          join pg in _db.Phieughidiens on c.MaPhieuGhi equals pg.MaPhieuGhi
                                          where c.MaBienBan == info.MaBienBan && pg.KyGhiChiSo == currentKyGhi
                                          select c).Any();
                if (isAlreadyRecorded)
                {
                    return;
                }
                MaBienBan = info.MaBienBan;
                TenKhachHang = info.HoTen;
                DiaChi = info.DiaChi;
                var hasLastReading = _db.Chitietghidiens.Any(c => c.MaBienBan == info.MaBienBan);

                ChiSoCu = hasLastReading
                    ? (from c in _db.Chitietghidiens
                       join pg in _db.Phieughidiens on c.MaPhieuGhi equals pg.MaPhieuGhi
                       where c.MaBienBan == info.MaBienBan
                       orderby pg.NgayGhi descending
                       select c.ChiSoMoi).FirstOrDefault()
                    : info.ChiSoBanDau;
            }
        }

        private void CalculateSanLuong()
        {
            if (!string.IsNullOrWhiteSpace(ChiSoMoiText) && !ChiSoMoi.HasValue)
            {
                SanLuongTieuThu = null;
                ErrorMessage = "Chỉ số mới phải là số nguyên.";
                return;
            }
            if (ChiSoMoi.HasValue)
            {
                if (ChiSoMoi.Value < 0)
                {
                    SanLuongTieuThu = null;
                    ErrorMessage = "Chỉ số mới không được là số âm.";
                }
                else if (ChiSoCu.HasValue)
                {
                    if (ChiSoMoi.Value >= ChiSoCu.Value)
                    {
                        SanLuongTieuThu = ChiSoMoi.Value - ChiSoCu.Value;
                        ErrorMessage = string.Empty;
                    }
                    else
                    {
                        SanLuongTieuThu = null;
                        ErrorMessage = "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ.";
                    }
                }
                else
                {
                    SanLuongTieuThu = null;
                    ErrorMessage = string.Empty;
                }
            }
            else
            {
                SanLuongTieuThu = null;
                ErrorMessage = string.Empty;
            }
        }
    }
}
