using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class TraCuuDeNghiViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        public Action RequestGoHome { get; set; }

        private string _maPhieu;
        private string _hoTen;
        private string _diaChi;
        private string _trangThai;
        private string _tuNgay;
        private string _denNgay;
        private string _selectedMucDich;
        private string _selectedSoPha;

        private string _stateMessage;
        private bool _isSuccessStatus;

        private List<Mucdich> _mucDichList;
        private List<Loaipha> _soPhaList;
        private ObservableCollection<PhieuSearchResult> _searchResults;

        public string MaPhieu { get => _maPhieu; set { _maPhieu = value; OnPropertyChanged(); } }
        public string HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(); } }
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(); } }
        public string TrangThai { get => _trangThai; set { _trangThai = value; OnPropertyChanged(); } }
        public string TuNgay { get => _tuNgay; set { _tuNgay = value; OnPropertyChanged(); } }
        public string DenNgay { get => _denNgay; set { _denNgay = value; OnPropertyChanged(); } }
        public string SelectedMucDich { get => _selectedMucDich; set { _selectedMucDich = value; OnPropertyChanged(); } }
        public string SelectedSoPha { get => _selectedSoPha; set { _selectedSoPha = value; OnPropertyChanged(); } }

        public string StateMessage { get => _stateMessage; set { _stateMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }

        public List<Mucdich> MucDichList { get => _mucDichList; set { _mucDichList = value; OnPropertyChanged(); } }
        public List<Loaipha> SoPhaList { get => _soPhaList; set { _soPhaList = value; OnPropertyChanged(); } }
        public ObservableCollection<PhieuSearchResult> SearchResults { get => _searchResults; set { _searchResults = value; OnPropertyChanged(); } }    

        public ICommand TraCuuCommand { get; }
        public ICommand ThoatCommand { get; }

        public TraCuuDeNghiViewModel(QuanLyCapDienContext context)
        {
            _db = context;

            TraCuuCommand = new RelayCommand(ExecuteTraCuu);
            ThoatCommand = new RelayCommand(ExecuteThoat);

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                var mucDichList = _db.Mucdiches.ToList();
                mucDichList.Insert(0, new Mucdich { MaMucDich = "", TenMucDich = "Tất cả" });
                MucDichList = mucDichList;
                SelectedMucDich = "";

                var soPhaList = _db.Loaiphas.ToList();
                soPhaList.Insert(0, new Loaipha { MaSoPha = "", TenSoPha = "Tất cả" });
                SoPhaList = soPhaList;
                SelectedSoPha = "";
            }
            catch (Exception ex)
            {
                StateMessage = "Lỗi khi tải dữ liệu từ hệ thống.";
                IsSuccessStatus = false;
                System.Diagnostics.Debug.WriteLine($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void ResetFormFields()
        {
            MaPhieu = string.Empty;
            HoTen = string.Empty;
            DiaChi = string.Empty;
            TrangThai = string.Empty;

            SelectedSoPha = "";
            SelectedMucDich = "";

            TuNgay = string.Empty;
            DenNgay = string.Empty;

            SearchResults?.Clear();

            StateMessage = string.Empty;
            IsSuccessStatus = true;
        }

        private void ExecuteTraCuu(object obj)
        {
            try
            {
                var query = from p in _db.Phieucapdiens
                            join lp in _db.Loaiphas on p.MaSoPha equals lp.MaSoPha
                            join md in _db.Mucdiches on p.MaMucDich equals md.MaMucDich
                            select new { p, lp, md };

                if (!string.IsNullOrWhiteSpace(MaPhieu))
                    query = query.Where(x => x.p.MaPhieu.Contains(MaPhieu.Trim()));

                if (!string.IsNullOrWhiteSpace(HoTen))
                    query = query.Where(x => x.p.HoTen.Contains(HoTen.Trim()));

                if (!string.IsNullOrWhiteSpace(DiaChi))
                    query = query.Where(x => x.p.DiaChi.Contains(DiaChi.Trim()));

                if (!string.IsNullOrWhiteSpace(TrangThai))
                    query = query.Where(x => x.p.TrangThai.Contains(TrangThai.Trim()));

                if (!string.IsNullOrEmpty(SelectedSoPha))
                    query = query.Where(x => x.p.MaSoPha == SelectedSoPha);

                if (!string.IsNullOrEmpty(SelectedMucDich))
                    query = query.Where(x => x.p.MaMucDich == SelectedMucDich);

                if (!string.IsNullOrWhiteSpace(TuNgay))
                {
                    if (DateTime.TryParseExact(TuNgay.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start))
                    {
                        query = query.Where(x => x.p.ThoiGianGui >= start);
                    }
                    else
                    {
                        query = query.Where(x => false);
                    }
                }

                if (!string.IsNullOrWhiteSpace(DenNgay))
                {
                    if (DateTime.TryParseExact(DenNgay.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime end))
                    {
                        end = end.AddDays(1);
                        query = query.Where(x => x.p.ThoiGianGui < end);
                    }
                    else
                    {
                        query = query.Where(x => false);
                    }
                }

                var rawData = query.OrderByDescending(x => x.p.ThoiGianGui).ToList();

                SearchResults = new ObservableCollection<PhieuSearchResult>(
                rawData.Select((item, index) => new PhieuSearchResult
                {
                    STT = index + 1,
                    ThoiGianGui = item.p.ThoiGianGui.ToString("dd/MM/yyyy HH:mm"),
                    HoTen = item.p.HoTen,
                    DiaChi = item.p.DiaChi,
                    SoPha = item.lp.TenSoPha,
                    MucDich = item.md.TenMucDich,
                    TrangThai = item.p.TrangThai
                })
                );
                if (SearchResults.Count == 0)
                {
                    StateMessage = "Không tìm thấy kết quả nào phù hợp.";
                    IsSuccessStatus = false;
                }
                else
                {
                    StateMessage = $"Đã tìm thấy {SearchResults.Count} kết quả.";
                    IsSuccessStatus = true;
                }
            }
            catch (Exception ex)
            {
                SearchResults?.Clear();

                StateMessage = "Đã xảy ra lỗi hệ thống khi tra cứu dữ liệu.";
                IsSuccessStatus = false;

                System.Diagnostics.Debug.WriteLine($"Lỗi Tra Cứu: {ex.Message}");
            }
        }

        private void ExecuteThoat(object obj)
        {
            ResetFormFields();
            RequestGoHome?.Invoke();
        }
        public override void Refresh()
        {
            try
            {
                var currentMucDich = SelectedMucDich;
                var currentSoPha = SelectedSoPha;

                var mucDichList = _db.Mucdiches.ToList();
                mucDichList.Insert(0, new Mucdich { MaMucDich = "", TenMucDich = "Tất cả" });
                MucDichList = mucDichList;

                var soPhaList = _db.Loaiphas.ToList();
                soPhaList.Insert(0, new Loaipha { MaSoPha = "", TenSoPha = "Tất cả" });
                SoPhaList = soPhaList;

                if (MucDichList.Any(m => m.MaMucDich == currentMucDich))
                {
                    SelectedMucDich = currentMucDich;
                }
                else
                {
                    SelectedMucDich = "";
                }

                if (SoPhaList.Any(s => s.MaSoPha == currentSoPha))
                {
                    SelectedSoPha = currentSoPha;
                }
                else
                {
                    SelectedSoPha = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải lại Tra cứu phiếu: {ex.Message}");
            }
        }
    } 
}
