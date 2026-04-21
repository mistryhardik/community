using MvvmHelpers;
using Xamarin.Forms;

namespace MyWeatherApp
{
    public class ViewModelBase : BaseViewModel
    {
        protected INavigation Navigation { get; }

        protected IDialogService DialogService { get; }

        public ViewModelBase(INavigation navigation, IDialogService dialogService)
        {
            Navigation = navigation;
            DialogService = dialogService;
        }

        public static void Init(bool mock = true)
        {

        }
    }
}
