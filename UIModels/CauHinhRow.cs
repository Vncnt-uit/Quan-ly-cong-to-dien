using Quản_lý_công_tơ_điện.Base;

namespace Quản_lý_công_tơ_điện.UIModels
{
    public class CauHinhRow : ObservableObject
    {
        private string _tenMucDich;

        public string MaMucDich { get; set; }
        public string MaSoPha { get; set; }
        public string TenSoPha { get; set; }

        public string OriginalName { get; set; }
        public Action<CauHinhRow> RequestSave { get; set; }

        public void SetInitialName(string name)
        {
            _tenMucDich = name;
            OriginalName = name;
            OnPropertyChanged(nameof(TenMucDich));
        }

        public void Revert()
        {
            _tenMucDich = OriginalName;
            OnPropertyChanged(nameof(TenMucDich));
        }

        public string TenMucDich
        {
            get => _tenMucDich;
            set
            {
                string cleanValue = value?.Trim();

                if (string.IsNullOrWhiteSpace(cleanValue))
                {
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Revert();
                    });
                    return;
                }

                if (_tenMucDich != cleanValue)
                {
                    _tenMucDich = cleanValue;
                    OnPropertyChanged();

                    RequestSave?.Invoke(this);
                }
            }
        }
    }
}
