using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using Quản_lý_công_tơ_điện.UIModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    class CauHinhHeThongViewModel : ObservableObject
    {
        private readonly QuanLyCapDienContext _db;

        private string _tenMucDichMoi;
        private QuyDinhGiaDien _selectedQuyDinhGia;
        private CauHinhRow _selectedMucDich;
        private CauHinhRow _selectedCauHinh;
        private CauHinhRow _selectedMucDichToAdd;
        private Loaipha _selectedLoaiPhaToAdd;

        private string _statusMessage;
        private bool _isSuccessStatus;
        private bool _hasTenMucDichError;
        private string _tenMucDichErrorMessage;

        private bool _isReloading = false;

        public QuyDinhGiaDien SelectedQuyDinhGia { get => _selectedQuyDinhGia; set { _selectedQuyDinhGia = value; OnPropertyChanged(); } }
        public CauHinhRow SelectedMucDich { get => _selectedMucDich; set { _selectedMucDich = value; OnPropertyChanged(); } }
        public CauHinhRow SelectedCauHinh { get => _selectedCauHinh; set { _selectedCauHinh = value; OnPropertyChanged(); } }
        public CauHinhRow SelectedMucDichToAdd { get => _selectedMucDichToAdd; set { _selectedMucDichToAdd = value; OnPropertyChanged(); ClearStatus(); } }
        public Loaipha SelectedLoaiPhaToAdd { get => _selectedLoaiPhaToAdd; set { _selectedLoaiPhaToAdd = value; OnPropertyChanged(); ClearStatus(); } }

        public string TenMucDichMoi { get => _tenMucDichMoi; set { _tenMucDichMoi = value; OnPropertyChanged(); ClearStatus(); ValidateTenMucDich(); } }

        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }
        public bool IsSuccessStatus { get => _isSuccessStatus; set { _isSuccessStatus = value; OnPropertyChanged(); } }
        public bool HasTenMucDichError { get => _hasTenMucDichError; set { _hasTenMucDichError = value; OnPropertyChanged(); } }
        public string TenMucDichErrorMessage { get => _tenMucDichErrorMessage; set { _tenMucDichErrorMessage = value; OnPropertyChanged(); } }

        public ObservableCollection<QuyDinhGiaDien> DanhSachQuyDinhGia { get; set; }
        public ObservableCollection<CauHinhRow> DanhSachMucDich { get; set; }
        public ObservableCollection<Loaipha> DanhSachLoaiPha { get; set; }
        public ObservableCollection<CauHinhRow> DanhSachCauHinh { get; set; }

        public ICommand ThemBacCommand { get; }
        public ICommand XoaBacCommand { get; }
        public ICommand LuuGiaDienCommand { get; }
        public ICommand ThemMucDichCommand { get; }
        public ICommand XoaMucDichCommand { get; }
        public ICommand ThemCauHinhCommand { get; }
        public ICommand XoaCauHinhCommand { get; }

        public CauHinhHeThongViewModel(QuanLyCapDienContext context)
        {
            _db = context;

            DanhSachQuyDinhGia = new ObservableCollection<QuyDinhGiaDien>();
            DanhSachMucDich = new ObservableCollection<CauHinhRow>();
            DanhSachLoaiPha = new ObservableCollection<Loaipha>();
            DanhSachCauHinh = new ObservableCollection<CauHinhRow>();

            ThemBacCommand = new RelayCommand(ExecuteThemBac);
            XoaBacCommand = new RelayCommand(ExecuteXoaBac, o => SelectedQuyDinhGia != null);
            LuuGiaDienCommand = new RelayCommand(ExecuteLuuGiaDien);

            ThemMucDichCommand = new RelayCommand(ExecuteThemMucDich, o => !HasTenMucDichError && !string.IsNullOrWhiteSpace(TenMucDichMoi));
            XoaMucDichCommand = new RelayCommand(ExecuteXoaMucDich, o => SelectedMucDich != null);

            ThemCauHinhCommand = new RelayCommand(ExecuteThemCauHinh, o => SelectedMucDichToAdd != null && SelectedLoaiPhaToAdd != null);
            XoaCauHinhCommand = new RelayCommand(ExecuteXoaCauHinh, o => SelectedCauHinh != null);

            PrepareNewForm();
        }

        private void PrepareNewForm()
        {
            TenMucDichMoi = string.Empty;
            HasTenMucDichError = false;
            TenMucDichErrorMessage = string.Empty;

            ClearStatus();
            LoadAllData();
        }

        private void ClearStatus()
        {
            if (_isReloading) return;

            if (!string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = string.Empty;
                IsSuccessStatus = true;
            }
        }

        private void LoadAllData()
        {
            _isReloading = true;
            try
            {
                DanhSachQuyDinhGia.Clear();
                var prices = _db.QuyDinhGiaDiens.OrderBy(p => p.Bac).ToList();
                foreach (var p in prices) DanhSachQuyDinhGia.Add(p);

                DanhSachMucDich.Clear();
                var purposes = _db.Mucdiches.ToList();
                foreach (var m in purposes)
                {
                    var row = new CauHinhRow { MaMucDich = m.MaMucDich };
                    row.SetInitialName(m.TenMucDich);
                    row.RequestSave = SaveCauHinhInlineEdit;
                    DanhSachMucDich.Add(row);
                }

                DanhSachLoaiPha.Clear();
                var phases = _db.Loaiphas.ToList();
                foreach (var l in phases) DanhSachLoaiPha.Add(l);

                LoadConstraintsGrid();
            }
            finally
            {
                _isReloading = false;
            }
        }

        private void LoadConstraintsGrid()
        {
            DanhSachCauHinh.Clear();

            var rawData = (from ch in _db.Cauhinhcapdiens
                           join md in _db.Mucdiches on ch.MaMucDich equals md.MaMucDich
                           join lp in _db.Loaiphas on ch.MaSoPha equals lp.MaSoPha
                           select new
                           {
                               ch.MaMucDich,
                               ch.MaSoPha,
                               md.TenMucDich,
                               lp.TenSoPha
                           }).ToList();

            foreach (var item in rawData)
            {
                var row = new CauHinhRow
                {
                    MaMucDich = item.MaMucDich,
                    MaSoPha = item.MaSoPha,
                    TenSoPha = item.TenSoPha
                };

                row.SetInitialName(item.TenMucDich);

                DanhSachCauHinh.Add(row);
            }
        }
        private void SaveCauHinhInlineEdit(CauHinhRow editedRow)
        {
            try
            {
                string normalizedName = editedRow.TenMucDich.ToLower();

                bool isDuplicate = _db.Mucdiches.Any(m => m.TenMucDich.ToLower() == normalizedName && m.MaMucDich != editedRow.MaMucDich);
                if (isDuplicate)
                {
                    IsSuccessStatus = false;
                    StatusMessage = $"* Mục đích '{editedRow.TenMucDich}' đã tồn tại.";
                    editedRow.Revert();
                    return;
                }

                var dbItem = _db.Mucdiches.FirstOrDefault(m => m.MaMucDich == editedRow.MaMucDich);
                if (dbItem != null)
                {
                    dbItem.TenMucDich = editedRow.TenMucDich;
                    _db.SaveChanges();

                    editedRow.OriginalName = editedRow.TenMucDich;

                    IsSuccessStatus = true;
                    StatusMessage = "CẬP NHẬT MỤC ĐÍCH THÀNH CÔNG!";

                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                IsSuccessStatus = false;
                StatusMessage = "LỖI CẬP NHẬT MỤC ĐÍCH.";
                editedRow.Revert();
                System.Diagnostics.Debug.WriteLine($"Lỗi Sửa Cấu Hình: {ex.Message}");
            }
        }

        private void ExecuteThemBac(object obj)
        {
            ClearStatus();

            var currentList = DanhSachQuyDinhGia.ToList();

            DanhSachQuyDinhGia.Clear();

            if (currentList.Any())
            {
                var currentLast = currentList.Last();
                if (currentLast.DinhMuc == null)
                {
                    currentLast.DinhMuc = 100;
                }
            }

            int newBac = currentList.Count + 1;
            currentList.Add(new QuyDinhGiaDien { Bac = newBac, DonGia = 1000, DinhMuc = null });

            foreach (var item in currentList)
            {
                DanhSachQuyDinhGia.Add(item);
            }
        }

        private void ExecuteXoaBac(object obj)
        {
            ClearStatus();
            if (DanhSachQuyDinhGia.Count <= 1)
            {
                IsSuccessStatus = false;
                StatusMessage = "HỆ THỐNG YÊU CẦU ÍT NHẤT 1 BẬC GIÁ ĐIỆN.";
                return;
            }
            DanhSachQuyDinhGia.Remove(SelectedQuyDinhGia);


            var currentList = DanhSachQuyDinhGia.ToList();
            DanhSachQuyDinhGia.Clear();
            for (int i = 0; i < currentList.Count; i++)
            {
                currentList[i].Bac = i + 1;

                if (i == currentList.Count - 1)
                {
                    currentList[i].DinhMuc = null;
                }

                DanhSachQuyDinhGia.Add(currentList[i]);
            }
        }

        private void ExecuteLuuGiaDien(object obj)
        {
            if (!DanhSachQuyDinhGia.Any()) return;

            var sortedList = DanhSachQuyDinhGia.OrderBy(x => x.Bac).ToList();
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (i == sortedList.Count - 1)
                {
                    sortedList[i].DinhMuc = null;
                }
                else if (sortedList[i].DinhMuc == null || sortedList[i].DinhMuc <= 0)
                {
                    sortedList[i].DinhMuc = 100;
                }
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.QuyDinhGiaDiens.RemoveRange(_db.QuyDinhGiaDiens);
                    _db.SaveChanges();

                    _db.QuyDinhGiaDiens.AddRange(sortedList);
                    _db.SaveChanges();

                    transaction.Commit();

                    IsSuccessStatus = true;
                    StatusMessage = "CẬP NHẬT QUY ĐỊNH THÀNH CÔNG!";
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    IsSuccessStatus = false;
                    StatusMessage = "LỖI ĐỒNG BỘ QUY ĐỊNH.";
                    System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
                }
            }
        }

        private void ValidateTenMucDich()
        {
            if (string.IsNullOrWhiteSpace(TenMucDichMoi))
            {
                HasTenMucDichError = true;
                TenMucDichErrorMessage = "* Vui lòng nhập tên mục đích.";
                return;
            }

            string cleanName = TenMucDichMoi.Trim();
            string normalizedName = cleanName.ToLower();

            bool isDuplicate = _db.Mucdiches.Any(m => m.TenMucDich.ToLower() == normalizedName);
            if (isDuplicate)
            {
                HasTenMucDichError = true;
                TenMucDichErrorMessage = $"* Mục đích '{cleanName}' đã tồn tại.";
                return;
            }

            HasTenMucDichError = false;
            TenMucDichErrorMessage = string.Empty;
        }

        private void ExecuteThemMucDich(object obj)
        {
            if (HasTenMucDichError || string.IsNullOrWhiteSpace(TenMucDichMoi)) return;

            try
            {
                string cleanName = TenMucDichMoi.Trim();
                int nextNumber = 1;
                var maxId = _db.Mucdiches.Select(m => m.MaMucDich).Max();

                if (!string.IsNullOrEmpty(maxId) && maxId.StartsWith("MD"))
                {
                    if (int.TryParse(maxId.Substring(2), out int currentMax))
                    {
                        nextNumber = currentMax + 1;
                    }
                }
                string newId = "MD" + nextNumber.ToString("D2");

                var newPurpose = new Mucdich { MaMucDich = newId, TenMucDich = cleanName };
                _db.Mucdiches.Add(newPurpose);
                _db.SaveChanges();

                TenMucDichMoi = string.Empty;
                HasTenMucDichError = false;
                TenMucDichErrorMessage = string.Empty;
                
                IsSuccessStatus = true;
                StatusMessage = "THÊM MỤC ĐÍCH THÀNH CÔNG!";
                LoadAllData();
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();

                IsSuccessStatus = false;
                StatusMessage = "LỖI THÊM MỤC ĐÍCH.";
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
            }
        }

        private void ExecuteXoaMucDich(object obj)
        {
            try
            {
                var associatedConstraints = _db.Cauhinhcapdiens.Where(c => c.MaMucDich == SelectedMucDich.MaMucDich);
                _db.Cauhinhcapdiens.RemoveRange(associatedConstraints);

                var entityToDelete = _db.Mucdiches.FirstOrDefault(m => m.MaMucDich == SelectedMucDich.MaMucDich);
                if (entityToDelete != null)
                {
                    _db.Mucdiches.Remove(entityToDelete);
                    _db.SaveChanges();
                }

                IsSuccessStatus = true;
                StatusMessage = "XÓA MỤC ĐÍCH THÀNH CÔNG!";
                LoadAllData();
            }
            catch (Exception)
            {
                _db.ChangeTracker.Clear();
                IsSuccessStatus = false;
                StatusMessage = "KHÔNG THỂ XÓA VÌ DỮ LIỆU ĐANG ĐƯỢC SỬ DỤNG.";
                LoadAllData();
            }
        }

        private void ExecuteThemCauHinh(object obj)
        {
            bool duplicate = _db.Cauhinhcapdiens.Any(c => c.MaMucDich == SelectedMucDichToAdd.MaMucDich && c.MaSoPha == SelectedLoaiPhaToAdd.MaSoPha);
            if (duplicate)
            {
                IsSuccessStatus = false;
                StatusMessage = "RÀNG BUỘC NÀY ĐÃ TỒN TẠI.";
                return;
            }

            try
            {
                var newRule = new Cauhinhcapdien
                {
                    MaMucDich = SelectedMucDichToAdd.MaMucDich,
                    MaSoPha = SelectedLoaiPhaToAdd.MaSoPha
                };

                _db.Cauhinhcapdiens.Add(newRule);
                _db.SaveChanges();

                IsSuccessStatus = true;
                StatusMessage = "THÊM CẤU HÌNH THÀNH CÔNG!";
                LoadConstraintsGrid();
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();

                IsSuccessStatus = false;
                StatusMessage = "LỖI THÊM CẤU HÌNH.";
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
            }
        }

        private void ExecuteXoaCauHinh(object obj)
        {
            try
            {
                var targetRule = _db.Cauhinhcapdiens.FirstOrDefault(c => c.MaMucDich == SelectedCauHinh.MaMucDich && c.MaSoPha == SelectedCauHinh.MaSoPha);
                if (targetRule != null)
                {
                    _db.Cauhinhcapdiens.Remove(targetRule);
                    _db.SaveChanges();

                    IsSuccessStatus = true;
                    StatusMessage = "XÓA CẤU HÌNH THÀNH CÔNG!";
                    LoadConstraintsGrid();
                }
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();

                IsSuccessStatus = false;
                StatusMessage = "LỖI XÓA CẤU HÌNH.";
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}