namespace Quản_lý_công_tơ_điện.Base
{
    public class BaseViewModel : ObservableObject, IRefreshable
    {
        public virtual void Refresh()
        {
        }
    }
}