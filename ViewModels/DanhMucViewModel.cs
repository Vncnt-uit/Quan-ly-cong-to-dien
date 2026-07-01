using Quản_lý_công_tơ_điện.Base;
using Quản_lý_công_tơ_điện.Models;
using System.Windows.Input;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class DanhMucViewModel : ObservableObject
    {
        private readonly QuanLyCapDienContext _db;
        private readonly TrangChuViewModel _trangChuViewModel;

        private readonly DeNghiCapDienViewModel _bm1ViewModel;
        private readonly BienBanCapDienViewModel _bm2ViewModel;
        private readonly TraCuuDeNghiViewModel _bm3ViewModel;
        private readonly PhieuGhiChiSoDienViewModel _bm4ViewModel;
        private readonly HoaDonTienDienViewModel _bm5ViewModel;
        private readonly ThongKeDienViewModel _bm6ViewModel;
        private readonly CauHinhHeThongViewModel _cauHinhHeThongViewModel;

        private ObservableObject _currentViewModel;
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
                
                if (_currentViewModel is IRefreshable refreshableVM)
                {
                    refreshableVM.Refresh();
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }
        private bool _isMenu1Checked;
        public bool IsMenu1Checked { get => _isMenu1Checked; set { _isMenu1Checked = value; OnPropertyChanged(); } }

        private bool _isMenu2Checked;
        public bool IsMenu2Checked { get => _isMenu2Checked; set { _isMenu2Checked = value; OnPropertyChanged(); } }

        private bool _isMenu3Checked;
        public bool IsMenu3Checked { get => _isMenu3Checked; set { _isMenu3Checked = value; OnPropertyChanged(); } }

        private bool _isMenu4Checked;
        public bool IsMenu4Checked { get => _isMenu4Checked; set { _isMenu4Checked = value; OnPropertyChanged(); } }

        private bool _isMenu5Checked;
        public bool IsMenu5Checked { get => _isMenu5Checked; set { _isMenu5Checked = value; OnPropertyChanged(); } }

        private bool _isMenu6Checked;
        public bool IsMenu6Checked { get => _isMenu6Checked; set { _isMenu6Checked = value; OnPropertyChanged(); } }

        private bool _isMenuCauHinhChecked;
        public bool IsMenuCauHinhChecked { get => _isMenuCauHinhChecked; set { _isMenuCauHinhChecked = value; OnPropertyChanged(); } }
        
        public ICommand NavigateBM1Command { get; }
        public ICommand NavigateBM2Command { get; }
        public ICommand NavigateBM3Command { get; }
        public ICommand NavigateBM4Command { get; }
        public ICommand NavigateBM5Command { get; }
        public ICommand NavigateBM6Command { get; }
        public ICommand NavigateCauHinhHeThongCommand { get; }


        public DanhMucViewModel(QuanLyCapDienContext context)
        {
            _db = context;
            _trangChuViewModel = new TrangChuViewModel();

            _bm1ViewModel = new DeNghiCapDienViewModel(_db);
            _bm2ViewModel = new BienBanCapDienViewModel(_db);
            _bm3ViewModel = new TraCuuDeNghiViewModel(_db);
            _bm4ViewModel = new PhieuGhiChiSoDienViewModel(_db);
            _bm5ViewModel = new HoaDonTienDienViewModel(_db);
            _bm6ViewModel = new ThongKeDienViewModel(_db);
            _cauHinhHeThongViewModel = new CauHinhHeThongViewModel(_db);

            _bm1ViewModel.RequestGoHome = GoHome;
            _bm2ViewModel.RequestGoHome = GoHome;
            _bm3ViewModel.RequestGoHome = GoHome;
            _bm4ViewModel.RequestGoHome = GoHome;
            _bm5ViewModel.RequestGoHome = GoHome;
            _bm6ViewModel.RequestGoHome = GoHome;

            NavigateBM1Command = new RelayCommand(ExecuteNavigateBM1, CanNavigateBM1);
            NavigateBM2Command = new RelayCommand(ExecuteNavigateBM2, CanNavigateBM2);
            NavigateBM3Command = new RelayCommand(ExecuteNavigateBM3, CanNavigateBM3);
            NavigateBM4Command = new RelayCommand(ExecuteNavigateBM4, CanNavigateBM4);
            NavigateBM5Command = new RelayCommand(ExecuteNavigateBM5, CanNavigateBM5);
            NavigateBM6Command = new RelayCommand(ExecuteNavigateBM6, CanNavigateBM6);
            NavigateCauHinhHeThongCommand = new RelayCommand(ExecuteNavigateCauHinhHeThong, CanNavigateCauHinhHeThong);

            CurrentViewModel = _trangChuViewModel;
        }
        private void GoHome()
        {
            CurrentViewModel = _trangChuViewModel;

            IsMenu1Checked = false;
            IsMenu2Checked = false;
            IsMenu3Checked = false;
            IsMenu4Checked = false;
            IsMenu5Checked = false;
            IsMenu6Checked = false;
            IsMenuCauHinhChecked = false;
        }
        private void ExecuteNavigateBM1(object obj)
        {
            CurrentViewModel = _bm1ViewModel;
        }
        private bool CanNavigateBM1(object obj)
        {
            return CurrentViewModel != _bm1ViewModel;
        }
        private void ExecuteNavigateBM2(object obj)
        {
            CurrentViewModel = _bm2ViewModel;
        }
        private bool CanNavigateBM2(object obj)
        {
            return CurrentViewModel != _bm2ViewModel;
        }
        private void ExecuteNavigateBM3(object obj)
        {
            CurrentViewModel = _bm3ViewModel;
        }
        private bool CanNavigateBM3(object obj)
        {
            return CurrentViewModel != _bm3ViewModel;
        }
        private void ExecuteNavigateBM4(object obj)
        {
            CurrentViewModel = _bm4ViewModel;
        }
        private bool CanNavigateBM4(object obj)
        {
            return CurrentViewModel != _bm4ViewModel;
        }
        private void ExecuteNavigateBM5(object obj)
        {
            CurrentViewModel = _bm5ViewModel;
        }
        private bool CanNavigateBM5(object obj)
        {
            return CurrentViewModel != _bm5ViewModel;
        }
        private void ExecuteNavigateBM6(object obj)
        {
            CurrentViewModel = _bm6ViewModel;
        }
        private bool CanNavigateBM6(object obj)
        {
            return CurrentViewModel != _bm6ViewModel;
        }
        private void ExecuteNavigateCauHinhHeThong(object obj)
        {
            CurrentViewModel = _cauHinhHeThongViewModel;
        }
        private bool CanNavigateCauHinhHeThong(object obj)
        {
            return CurrentViewModel != _cauHinhHeThongViewModel;
        }
    }
}
