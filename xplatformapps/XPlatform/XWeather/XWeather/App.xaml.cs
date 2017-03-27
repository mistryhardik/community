using Xamarin.Forms;
using XWeather.Views;

namespace XWeather
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // this will start a new instance of Main Page
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
