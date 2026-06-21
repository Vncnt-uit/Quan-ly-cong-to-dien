using Quản_lý_công_tơ_điện.Helpers;
using Quản_lý_công_tơ_điện.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class ThongKeRowViewModel
    {
        public int STT { get; set; }
        public string ThangText => $"Tháng {Thang}";
        public int Thang { get; set; }
        public int SanLuong { get; set; }
        public string SoVoiThangTruoc { get; set; }
    }

    class ThongKeDienViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        private int? _selectedNam;
        private string _statusMessage;
        private bool _isSuccessStatus;
        private bool _hasNamError;
        private string _namErrorMessage;

        public int? SelectedNam
        {
            get => _selectedNam;
            set { _selectedNam = value; OnPropertyChanged(); ClearStatus(); ValidateNam(); }
        }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }

        public bool HasNamError { get => _hasNamError; set { _hasNamError = value; OnPropertyChanged(); } }
        public string NamErrorMessage { get => _namErrorMessage; set { _namErrorMessage = value; OnPropertyChanged(); } }

        public ObservableCollection<int> DanhSachNam { get; set; }
        public ObservableCollection<ThongKeRowViewModel> DanhSachThongKe { get; set; }

        public ICommand ThongKeCommand { get; }
        public ICommand ResetNamCommand { get; }

        public ThongKeDienViewModel(QuanLyCapDienContext context)
        {
            _db = context;
            DanhSachThongKe = new ObservableCollection<ThongKeRowViewModel>();

            int currentYear = DateTime.Now.Year;
            DanhSachNam = new ObservableCollection<int>(Enumerable.Range(currentYear - 5, 6).Reverse());

            ThongKeCommand = new RelayCommand(ExecuteThongKe, CanExecuteThongKe);
            ResetNamCommand = new RelayCommand(ExecuteResetNam);

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            SelectedNam = DateTime.Now.Year;
            DanhSachThongKe.Clear();

            HasNamError = false;
            NamErrorMessage = string.Empty;
            ClearStatus();
        }

        private void ClearStatus()
        {
            if (!string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = string.Empty;
                IsSuccessStatus = true;
            }
        }

        private void ValidateNam()
        {
            if (!SelectedNam.HasValue)
            {
                HasNamError = true;
                NamErrorMessage = "* Vui lòng chọn năm thống kê.";
            }
            else
            {
                HasNamError = false;
                NamErrorMessage = string.Empty;
            }
        }

        private bool CanExecuteThongKe(object obj)
        {
            return SelectedNam.HasValue && !HasNamError;
        }

        private void ExecuteThongKe(object obj)
        {
            GenerateStatistics();
        }

        private void ExecuteResetNam(object obj)
        {
            PrepareNewForm();
        }

        private void GenerateStatistics()
        {
            try
            {
                DanhSachThongKe.Clear();
                if (!SelectedNam.HasValue) return;

                int nam = SelectedNam.Value;
                string currentYearSuffix = $"/{nam}";
                string decPrevYear = $"12/{nam - 1}";

                var rawData = (from c in _db.Chitietghidiens
                               join p in _db.Phieughidiens on c.MaPhieuGhi equals p.MaPhieuGhi
                               where p.KyGhiChiSo.EndsWith(currentYearSuffix) || p.KyGhiChiSo == decPrevYear
                               select new { p.KyGhiChiSo, c.SanLuongTieuThu }).ToList();

                var monthlyTotals = rawData.GroupBy(x => x.KyGhiChiSo)
                                           .ToDictionary(g => g.Key, g => g.Sum(x => x.SanLuongTieuThu));

                for (int i = 1; i <= 12; i++)
                {
                    string currentMonthStr = $"{i:D2}/{nam}";
                    string prevMonthStr = i == 1 ? $"12/{nam - 1}" : $"{(i - 1):D2}/{nam}";

                    int currentTotal = monthlyTotals.ContainsKey(currentMonthStr) ? monthlyTotals[currentMonthStr] : 0;
                    int prevTotal = monthlyTotals.ContainsKey(prevMonthStr) ? monthlyTotals[prevMonthStr] : 0;

                    string percentageStr;

                    if (currentTotal == 0 && prevTotal == 0)
                    {
                        percentageStr = "-";
                    }
                    else if (prevTotal == 0)
                    {
                        percentageStr = "+100%";
                    }
                    else
                    {
                        double variance = ((double)(currentTotal - prevTotal) / prevTotal) * 100;
                        string sign = variance > 0 ? "+" : "";
                        percentageStr = $"{sign}{Math.Round(variance, 2)}%";
                    }

                    DanhSachThongKe.Add(new ThongKeRowViewModel
                    {
                        STT = i,
                        Thang = i,
                        SanLuong = currentTotal,
                        SoVoiThangTruoc = percentageStr
                    });
                }

                IsSuccessStatus = true;
                StatusMessage = $"THỐNG KÊ NĂM {nam} THÀNH CÔNG!";
            }
            catch (Exception ex)
            {
                IsSuccessStatus = false;
                StatusMessage = "CÓ LỖI XẢY RA KHI TẢI DỮ LIỆU.";
                System.Diagnostics.Debug.WriteLine($"Lỗi thống kê: {ex.Message}");
            }
        }
        public override void Refresh()
        {
            try
            {
                if (SelectedNam.HasValue && !HasNamError && DanhSachThongKe.Count > 0)
                {
                    GenerateStatistics();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi refresh BM6: {ex.Message}");
            }
        }
    }
}