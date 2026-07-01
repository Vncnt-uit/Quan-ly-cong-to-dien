using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class DeNghiCapDienViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        public Action RequestGoHome { get; set; }

        private string _cccd;
        private string _hoTen;
        private int? _namSinh;
        private string _soDienThoai;
        private string _email;
        private string _diaChi;
        private string _maYeuCau;
        private DateTime? _thoiGian;

        private string _selectedMucDich;
        private string _selectedSoPha;

        private bool _hasCccdError;
        private string _cccdErrorMessage;
        
        private bool _hasHoTenError;
        private string _hoTenErrorMessage;

        private bool _hasNamSinhError;
        private string _namSinhErrorMessage;

        private bool _hasSdtError;
        private string _sdtErrorMessage;

        private bool _hasEmailError;
        private string _emailErrorMessage;

        private bool _hasDiaChiError;
        private string _diaChiErrorMessage;

        private bool _hasMucDichError;
        private string _mucDichErrorMessage;

        private bool _hasSoPhaError;
        private string _soPhaErrorMessage;

        private string _statusMessage;
        private bool _isSuccessStatus;

        private List<Mucdich> _mucDichList;
        private List<Loaipha> _soPhaList;

        public string Cccd { get => _cccd; set { _cccd = value; OnPropertyChanged(); ClearStatus(); ValidateCccd(); } }
        public string HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); ClearStatus(); ValidateHoTen(); } }
        public string SoDienThoai { get => _soDienThoai; set { _soDienThoai = value; OnPropertyChanged(); ClearStatus(); ValidateSdt(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); ClearStatus(); ValidateEmail(); } }
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); ClearStatus(); ValidateDiaChi();  } }
        public int? NamSinh
        {
            get => _namSinh;
            set
            {
                _namSinh = value;
                OnPropertyChanged();
                ClearStatus();
                ValidateNamSinh();
            }
        }
        public string SelectedMucDich
        {
            get => _selectedMucDich;
            set
            {
                _selectedMucDich = value;
                OnPropertyChanged();
                ClearStatus();
                LoadSoPhaList();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    HasMucDichError = false;
                    MucDichErrorMessage = string.Empty;
                }
            }
        }
        public string SelectedSoPha
        {
            get => _selectedSoPha;
            set
            {
                _selectedSoPha = value;
                OnPropertyChanged();
                ClearStatus();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    HasSoPhaError = false;
                    SoPhaErrorMessage = string.Empty;
                }
            }
        }
        public string MaYeuCau { get => _maYeuCau; set { _maYeuCau = value; OnPropertyChanged(); } }
        public DateTime? ThoiGian { get => _thoiGian; set { _thoiGian = value; OnPropertyChanged(); } }

        public bool HasCccdError { get => _hasCccdError; set { _hasCccdError = value; OnPropertyChanged(); } }
        public string CccdErrorMessage { get => _cccdErrorMessage; set { _cccdErrorMessage = value; OnPropertyChanged(); } }
        
        public bool HasHoTenError { get => _hasHoTenError; set { _hasHoTenError = value; OnPropertyChanged(); } }
        public string HoTenErrorMessage { get => _hoTenErrorMessage; set { _hoTenErrorMessage = value; OnPropertyChanged(); } }

        public bool HasNamSinhError { get => _hasNamSinhError; set { _hasNamSinhError = value; OnPropertyChanged(); } }
        public string NamSinhErrorMessage { get => _namSinhErrorMessage; set { _namSinhErrorMessage = value; OnPropertyChanged(); } }

        public bool HasSdtError { get => _hasSdtError; set { _hasSdtError = value; OnPropertyChanged(); } }
        public string SdtErrorMessage { get => _sdtErrorMessage; set { _sdtErrorMessage = value; OnPropertyChanged(); } }

        public bool HasEmailError { get => _hasEmailError; set { _hasEmailError = value; OnPropertyChanged(); } }
        public string EmailErrorMessage { get => _emailErrorMessage; set { _emailErrorMessage = value; OnPropertyChanged(); } }

        public bool HasDiaChiError { get => _hasDiaChiError; set { _hasDiaChiError = value; OnPropertyChanged(); } }
        public string DiaChiErrorMessage { get => _diaChiErrorMessage; set { _diaChiErrorMessage = value; OnPropertyChanged(); } }

        public bool HasMucDichError { get => _hasMucDichError; set { _hasMucDichError = value; OnPropertyChanged(); }  }
        public string MucDichErrorMessage { get => _mucDichErrorMessage; set { _mucDichErrorMessage = value; OnPropertyChanged(); }  }

        public bool HasSoPhaError { get => _hasSoPhaError; set { _hasSoPhaError = value; OnPropertyChanged(); } }
        public string SoPhaErrorMessage { get => _soPhaErrorMessage; set { _soPhaErrorMessage = value; OnPropertyChanged(); } }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }
        
        public List<Mucdich> MucDichList { get => _mucDichList; set { _mucDichList = value; OnPropertyChanged(); } }
        public List<Loaipha> SoPhaList { get => _soPhaList; set { _soPhaList = value; OnPropertyChanged(); } }

        public ICommand GuiCommand { get; }
        public ICommand ThoatCommand { get; }

        public DeNghiCapDienViewModel(QuanLyCapDienContext context)
        {
            _db = context;

            GuiCommand = new RelayCommand(ExecuteGui);
            ThoatCommand = new RelayCommand(ExecuteThoat);

            LoadInitialData();
        }
        private void ValidateCccd()
        {
            if (string.IsNullOrWhiteSpace(Cccd))
            {
                HasCccdError = true;
                CccdErrorMessage = "* Vui lòng nhập số CCCD.";
                return;
            }

            if (!Regex.IsMatch(Cccd.Trim(), @"^\d{12}$"))
            {
                HasCccdError = true;
                CccdErrorMessage = "* Số CCCD không hợp lệ.";
                return;
            }

            if (_db.Phieucapdiens.Any(p => p.Cccd == Cccd.Trim()))
            {
                HasCccdError = true;
                CccdErrorMessage = "* Số CCCD này đã được đăng ký trước đó!";
                return;
            }

            HasCccdError = false;
            CccdErrorMessage = string.Empty;
        }
        private void ValidateHoTen()
        {
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                HasHoTenError = true;
                HoTenErrorMessage = "* Vui lòng nhập họ tên.";
                return;
            }
            string trimmedName = HoTen.Trim();

            if (trimmedName.Length > 100)
            {
                HasHoTenError = true;
                HoTenErrorMessage = "* Họ tên không được vượt quá 100 ký tự.";
                return;
            }
            if (!Regex.IsMatch(trimmedName, @"^[\p{L}\s]+$"))
            {
                HasHoTenError = true;
                HoTenErrorMessage = "* Họ tên không hợp lệ.";
                return;
            }    
            HasHoTenError = false;
            HoTenErrorMessage = string.Empty;
        }
        private void ValidateNamSinh()
        {
            if (!NamSinh.HasValue)
            {
                HasNamSinhError = true;
                NamSinhErrorMessage = "* Vui lòng nhập năm sinh.";
                return;
            }

            if (NamSinh < 1900 || NamSinh > DateTime.Now.Year)
            {
                HasNamSinhError = true;
                NamSinhErrorMessage = $"* Năm sinh không hợp lệ. (1900 - {DateTime.Now.Year})";
                return;
            }

            HasNamSinhError = false;
            NamSinhErrorMessage = string.Empty;
        }
        private void ValidateSdt()
        {
            if (string.IsNullOrWhiteSpace(SoDienThoai))
            {
                HasSdtError = true;
                SdtErrorMessage = "* Vui lòng nhập số điện thoại.";
                return;
            }
            
            if (!Regex.IsMatch(SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                HasSdtError = true;
                SdtErrorMessage = "* Số điện thoại không hợp lệ.";
                return;
            }
            HasSdtError = false;
            SdtErrorMessage = string.Empty;
        }
        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                HasEmailError = true;
                EmailErrorMessage = "* Vui lòng nhập địa chỉ email.";
                return;
            }
            string trimmedEmail = Email.Trim();

            if (trimmedEmail.Length > 100)
            {
                HasEmailError = true;
                EmailErrorMessage = "* Email không được vượt quá 100 ký tự.";
                return;
            }
            if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                HasEmailError = true;
                EmailErrorMessage = "* Địa chỉ email không hợp lệ.";
                return;
            }
            if (_db.Phieucapdiens.Any(p => p.Email == Email.Trim()))
            {
                HasEmailError = true;
                EmailErrorMessage = "* Địa chỉ email này đã được đăng ký trước đó!";
                return;
            }
            HasEmailError = false;
            EmailErrorMessage = string.Empty;
        }
        private void ValidateDiaChi()
        {
            if (string.IsNullOrWhiteSpace(DiaChi))
            {
                HasDiaChiError = true;
                DiaChiErrorMessage = "* Vui lòng nhập địa chỉ.";
                return;
            }

            if (DiaChi.Trim().Length > 255)
            {
                HasDiaChiError = true;
                DiaChiErrorMessage = "* Địa chỉ không được vượt quá 255 ký tự.";
                return;
            }

            HasDiaChiError = false;
            DiaChiErrorMessage = string.Empty;
        }
        private void ValidateComboBoxes()
        {
            if (string.IsNullOrWhiteSpace(SelectedMucDich))
            {
                HasMucDichError = true;
                MucDichErrorMessage = "* Vui lòng chọn mục đích.";
            }
            else
            {
                HasMucDichError = false;
                MucDichErrorMessage = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(SelectedSoPha))
            {
                HasSoPhaError = true;
                SoPhaErrorMessage = "* Vui lòng chọn số pha.";
            }
            else
            {
                HasSoPhaError = false;
                SoPhaErrorMessage = string.Empty;
            }
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
                MucDichList = _db.Mucdiches.ToList();
            }
            catch (Exception ex)
            {
                IsSuccessStatus = false;
                StatusMessage = "Lỗi kết nối CSDL: Không tải được mục đích sử dụng.";
                System.Diagnostics.Debug.WriteLine($"Lỗi tải dữ liệu: {ex.Message}");
            }

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            ThoiGian = DateTime.Today;
            GenerateMaYeuCau();

            HasCccdError = false;
            HasEmailError = false;
            HasSdtError = false;
            HasHoTenError = false;
            HasDiaChiError = false;
            HasNamSinhError = false;
            HasMucDichError = false;
            HasSoPhaError = false;

            CccdErrorMessage = string.Empty;
            EmailErrorMessage = string.Empty;
            SdtErrorMessage = string.Empty;
            HoTenErrorMessage = string.Empty;
            DiaChiErrorMessage = string.Empty;
            NamSinhErrorMessage = string.Empty;
            MucDichErrorMessage = string.Empty;
            SoPhaErrorMessage = string.Empty;
        }

        private void GenerateMaYeuCau()
        {
            try
            {
                var lastRecord = _db.Phieucapdiens
                                    .OrderByDescending(p => p.MaPhieu.Length)
                                    .ThenByDescending(p => p.MaPhieu)
                                    .FirstOrDefault();
                if (lastRecord != null)
                {
                    string lastId = lastRecord.MaPhieu;
                    int nextNum = int.Parse(lastId.Substring(3)) + 1;
                    MaYeuCau = "PCD" + nextNum.ToString("D3");
                }
                else
                {
                    MaYeuCau = "PCD001";
                }
            }
            catch
            {
                MaYeuCau = "PCD_ERR";
            }
        }

        private void LoadSoPhaList()
        {
            SoPhaList = null;
            SelectedSoPha = null;

            if (!string.IsNullOrEmpty(SelectedMucDich))
            {
                SoPhaList = _db.Cauhinhcapdiens
                    .Where(c => c.MaMucDich == SelectedMucDich)
                    .Select(c => c.MaSoPhaNavigation)
                    .ToList();

                if (SoPhaList.Count > 0)
                {
                    SelectedSoPha = SoPhaList[0].MaSoPha;
                }
            }
        }

        private void ResetFormFields()
        {
            Cccd = string.Empty;
            HoTen = string.Empty;
            NamSinh = null;
            SoDienThoai = string.Empty;
            Email = string.Empty;
            DiaChi = string.Empty;
            SelectedMucDich = null;

            StatusMessage = string.Empty;
            IsSuccessStatus = true;

            PrepareNewForm();
        }

        private void ExecuteThoat(object obj)
        {
            ResetFormFields();
            RequestGoHome?.Invoke();
        }

        private void ExecuteGui(object obj)
        {
            ValidateCccd();
            ValidateHoTen();
            ValidateNamSinh();
            ValidateSdt();
            ValidateEmail();
            ValidateDiaChi();
            ValidateComboBoxes();
            
            if (HasCccdError || HasHoTenError || HasNamSinhError || HasSdtError ||
                HasEmailError || HasDiaChiError || HasMucDichError || HasSoPhaError)
            {
                IsSuccessStatus = false;
                StatusMessage = "Vui lòng kiểm tra lại các thông tin chưa hợp lệ.";
                return;
            }
            try
            {
                string formattedName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(HoTen.Trim().ToLower());

                var newPhieu = new Phieucapdien
                {
                    MaPhieu = MaYeuCau,
                    ThoiGianGui = ThoiGian ?? DateTime.Today,
                    Cccd = Cccd.Trim(),
                    HoTen = formattedName,
                    NamSinh = NamSinh,
                    SoDienThoai = SoDienThoai,
                    Email = Email,
                    DiaChi = DiaChi,
                    MaMucDich = SelectedMucDich,
                    MaSoPha = SelectedSoPha,
                    TrangThai = "Chưa xử lý"
                };

                _db.Phieucapdiens.Add(newPhieu);
                _db.SaveChanges();

                ResetFormFields();

                IsSuccessStatus = true;
                StatusMessage = $"ĐÃ GỬI ĐỀ NGHỊ THÀNH CÔNG!";
            }
            catch (Exception ex)
            {
                IsSuccessStatus = false;
                StatusMessage = $"CÓ LỖI XẢY RA KHI GỬI ĐỀ NGHỊ!";

                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi đề nghị: {ex.Message}");
            }
        }
        public override void Refresh()
        {
            try
            {
                MucDichList = _db.Mucdiches.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi tải lại Phiếu đề nghị: {ex.Message}");
            }
        }
    }
}
