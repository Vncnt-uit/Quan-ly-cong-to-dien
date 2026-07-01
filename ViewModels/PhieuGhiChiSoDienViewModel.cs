using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using Quản_lý_công_tơ_điện.UIModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class PhieuGhiChiSoDienViewModel : BaseViewModel
    {
        private readonly QuanLyCapDienContext _db;

        public Action RequestGoHome { get; set; }

        private string _maPhieuGhi;
        private string _kyGhiChiSo;
        private string _ngayGhi;
        private string _nhanVienGhi;

        private bool _hasNhanVienGhiError;
        private string _nhanVienGhiErrorMessage;

        private string _statusMessage;
        private bool _isSuccessStatus;

        public string MaPhieuGhi { get => _maPhieuGhi; set { _maPhieuGhi = value; OnPropertyChanged(); } }
        public string KyGhiChiSo { get => _kyGhiChiSo; set { _kyGhiChiSo = value; OnPropertyChanged(); } }
        public string NgayGhi { get => _ngayGhi; set { _ngayGhi = value; OnPropertyChanged(); } }
        public string NhanVienGhi { get => _nhanVienGhi; set { _nhanVienGhi = value; OnPropertyChanged(); ClearStatus(); ValidateTenNhanVien(); } }

        public bool HasNhanVienGhiError { get => _hasNhanVienGhiError; set { _hasNhanVienGhiError = value; OnPropertyChanged(); } }
        public string NhanVienGhiErrorMessage { get => _nhanVienGhiErrorMessage; set { _nhanVienGhiErrorMessage = value; OnPropertyChanged(); } }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }

        public ObservableCollection<ChiTietGhiDienRow> DanhSachGhiDien { get; set; }

        public ICommand LuuCommand { get; }
        public ICommand ThoatCommand { get; }

        public PhieuGhiChiSoDienViewModel(QuanLyCapDienContext context)
        {
            _db = context;
            DanhSachGhiDien = new ObservableCollection<ChiTietGhiDienRow>();

            LuuCommand = new RelayCommand(ExecuteLuu);
            ThoatCommand = new RelayCommand(ExecuteThoat);

            PrepareNewForm();
        }
        private void ClearStatus()
        {
            if (!string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = string.Empty;
            }
        }
        private void PrepareNewForm()
        {
            KyGhiChiSo = DateTime.Now.ToString("MM/yyyy");
            NgayGhi = DateTime.Today.ToString("dd/MM/yyyy");

            HasNhanVienGhiError = false;
            NhanVienGhiErrorMessage = string.Empty;

            DanhSachGhiDien.Clear();
            AddEmptyRow();
        }
        private string GenerateMaPhieuGhi()
        {
            try
            {
                var lastRecord = _db.Phieughidiens.OrderByDescending(p => p.MaPhieuGhi).FirstOrDefault();
                if (lastRecord != null && lastRecord.MaPhieuGhi.StartsWith("PGD"))
                {
                    int nextNum = int.Parse(lastRecord.MaPhieuGhi.Substring(3)) + 1;
                    return "PGD" + nextNum.ToString("D3");
                }
                return "PGD001";
            }
            catch
            {
                return "PGD_ERR";
            }
        }

        private void AddEmptyRow()
        {
            if (DanhSachGhiDien.Count > 0 && !DanhSachGhiDien.Last().IsValidRow) return;

            var newRow = new ChiTietGhiDienRow(_db, DanhSachGhiDien.Count + 1)
            {
                RequestValidation = ValidateGrid
            };
            DanhSachGhiDien.Add(newRow);
        }

        private void ValidateTenNhanVien()
        {
            if (string.IsNullOrWhiteSpace(NhanVienGhi))
            {
                HasNhanVienGhiError = true;
                NhanVienGhiErrorMessage = "* Vui lòng nhập tên nhân viên.";
            }
            else if (NhanVienGhi.Length > 100)
            {
                HasNhanVienGhiError = true;
                NhanVienGhiErrorMessage = "* Tên nhân viên tối đa 100 ký tự.";
            }
            else if (!Regex.IsMatch(NhanVienGhi, @"^[\p{L}\s]+$"))
            {
                HasNhanVienGhiError = true;
                NhanVienGhiErrorMessage = "* Tên nhân viên không hợp lệ.";
            }
            else
            {
                HasNhanVienGhiError = false;
                NhanVienGhiErrorMessage = string.Empty;
            }
        }

        private void ValidateGrid()
        {
            StatusMessage = string.Empty;
            IsSuccessStatus = true;

            for (int i = DanhSachGhiDien.Count - 2; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(DanhSachGhiDien[i].MaCongTo))
                {
                    DanhSachGhiDien[i].RequestValidation = null;
                    DanhSachGhiDien.RemoveAt(i);
                }
            }
            for (int i = 0; i < DanhSachGhiDien.Count; i++)
            {
                DanhSachGhiDien[i].STT = i + 1;
            }
            foreach (var row in DanhSachGhiDien)
            {
                row.IsBlockedByParent = false;
            }
            var filledRows = DanhSachGhiDien.Where(r => !string.IsNullOrWhiteSpace(r.MaCongTo)).ToList();
            if (!filledRows.Any())
            {
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                return;
            }

            var duplicatesInGrid = filledRows.GroupBy(r => r.MaCongTo.Trim().ToUpper()).Where(g => g.Count() > 1).ToList();
            if (duplicatesInGrid.Any())
            {
                string dupId = duplicatesInGrid.First().Key;

                var badRowsList = filledRows.Where(r => r.MaCongTo.Trim().ToUpper() == dupId).ToList();
                foreach (var badRow in badRowsList) badRow.IsBlockedByParent = true;

                var badRowsNums = badRowsList.Select(r => r.STT);
                SetError($"Mã công tơ {dupId} bị nhập trùng ở các hàng {string.Join(", ", badRowsNums)}.");
                return;
            }

            var enteredMeters = filledRows.Select(r => r.MaCongTo.Trim().ToUpper()).Distinct().ToList();
            var realMetersInDb = _db.Bienbancapdiens.Where(b => enteredMeters.Contains(b.MaCongTo.Trim().ToUpper())).Select(b => b.MaCongTo.Trim().ToUpper()).ToList();
            var fakeMeters = enteredMeters.Except(realMetersInDb).ToList();

            if (fakeMeters.Any())
            {
                var badRowsList = filledRows.Where(r => fakeMeters.Contains(r.MaCongTo.Trim().ToUpper())).ToList();
                foreach (var badRow in badRowsList) badRow.IsBlockedByParent = true;

                var badRowsNums = badRowsList.Select(r => r.STT);
                SetError($"Mã công tơ trong hàng {string.Join(", ", badRowsNums)} không tồn tại.");
                return;
            }

            var duplicatesInDb = (from c in _db.Chitietghidiens
                                  join b in _db.Bienbancapdiens on c.MaBienBan equals b.MaBienBan
                                  join p in _db.Phieughidiens on c.MaPhieuGhi equals p.MaPhieuGhi
                                  where p.KyGhiChiSo == this.KyGhiChiSo &&
                                  enteredMeters.Contains(b.MaCongTo.Trim().ToUpper())
                                  select b.MaCongTo.Trim().ToUpper()).ToList();

            if (duplicatesInDb.Any())
            {
                var badRowsList = filledRows.Where(r => duplicatesInDb.Contains(r.MaCongTo.Trim().ToUpper())).ToList();
                foreach (var badRow in badRowsList) badRow.IsBlockedByParent = true;

                var badRowsNums = badRowsList.Select(r => r.STT);
                SetError($"Mã công tơ trong hàng {string.Join(", ", badRowsNums)} đã được ghi trong kỳ này.");
                return;
            }

            foreach (var row in filledRows)
            {
                if (row.ChiSoCu == null) row.TriggerAutoFill();
            }

            var firstErrorRow = filledRows.FirstOrDefault(r => !string.IsNullOrEmpty(r.ErrorMessage));
            if (firstErrorRow != null)
            {
                SetError($"Lỗi dữ liệu ở hàng {firstErrorRow.STT}: {firstErrorRow.ErrorMessage}");
                return;
            }

            if (DanhSachGhiDien.Any() && DanhSachGhiDien.Last().IsValidRow)
            {
                AddEmptyRow();
            }

            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private void SetError(string message)
        {
            StatusMessage = message;
            IsSuccessStatus = false;
        }

        private void ResetFormFields()
        {
            NhanVienGhi = string.Empty;
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
            ValidateTenNhanVien();
            ValidateGrid();
            var validRows = DanhSachGhiDien.Where(r => r.IsValidRow).ToList();
            if (HasNhanVienGhiError || !string.IsNullOrEmpty(StatusMessage))
            {
                if (string.IsNullOrEmpty(StatusMessage))
                {
                    SetError("Vui lòng kiểm tra lại tất cả thông tin.");
                }
                return;
            }
            if (!validRows.Any())
            {
                SetError("Vui lòng nhập tất cả thông tin ghi điện hợp lệ.");
                return;
            }
            try
            {
                string formattedName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(NhanVienGhi.Trim().ToLower());

                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        string newMaPhieuGhi = GenerateMaPhieuGhi();
                        var phieu = new Phieughidien
                        {
                            MaPhieuGhi = newMaPhieuGhi,
                            KyGhiChiSo = KyGhiChiSo,
                            NgayGhi = DateTime.Today,
                            NhanVienGhi = formattedName
                        };
                        _db.Phieughidiens.Add(phieu);

                        foreach (var row in validRows)
                        {
                            var chiTiet = new Chitietghidien
                            {
                                MaPhieuGhi = newMaPhieuGhi,
                                MaBienBan = row.MaBienBan,
                                ChiSoCu = row.ChiSoCu.Value,
                                ChiSoMoi = row.ChiSoMoi.Value,
                                SanLuongTieuThu = row.SanLuongTieuThu.Value
                            };
                            _db.Chitietghidiens.Add(chiTiet);
                        }

                        _db.SaveChanges();
                        transaction.Commit();

                        ResetFormFields();

                        StatusMessage = "LƯU PHIẾU GHI CHỈ SỐ THÀNH CÔNG!";
                        IsSuccessStatus = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        StatusMessage = "ĐÃ XẢY RA LỖI TRONG QUÁ TRÌNH LƯU PHIẾU.";
                        IsSuccessStatus = false;
                        System.Diagnostics.Debug.WriteLine($"Lỗi Database (Lưu Phiếu ghi điện): {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "ĐÃ XẢY RA LỖI HỆ THỐNG.";
                IsSuccessStatus = false;
                System.Diagnostics.Debug.WriteLine($"Lỗi Hệ Thống (Lưu Phiếu ghi điện): {ex.Message}");
            }
        }
    }
}
