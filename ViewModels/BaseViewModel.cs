using Quản_lý_công_tơ_điện.Helpers;

namespace Quản_lý_công_tơ_điện.ViewModels
{
    public class BaseViewModel : ObservableObject, IRefreshable
    {
        public virtual void Refresh()
        {
        }
    }
}