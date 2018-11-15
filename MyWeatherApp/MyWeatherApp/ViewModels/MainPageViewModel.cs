using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyWeatherApp.Models;
using MyWeatherApp.Repository;
using Xamarin.Forms;

namespace MyWeatherApp
{
    public class MainPageViewModel : ViewModelBase
    {
        Color mBackgroundColor;
        public Color BackgroundColor
        {
            get { return mBackgroundColor; }
            set { SetProperty(ref mBackgroundColor, value); }
        }

        Color mTextColor;
        public Color TextColor
        {
            get { return mTextColor; }
            set { SetProperty(ref mTextColor, value); }
        }

        string mCity = "City";
        public string City
        {
            get { return mCity; }
            set { SetProperty(ref mCity, value); }
        }

        string mCityName;
        public string CityName
        {
            get { return mCityName; }
            set { SetProperty(ref mCityName, value); }
        }

        double mTempreature = 0;
        public double Tempreature
        {
            get { return mTempreature; }
            set { SetProperty(ref mTempreature, value); }
        }

        string mCondition = "Condition";
        public string Condition
        {
            get { return mCondition; }
            set { SetProperty(ref mCondition, value); }
        }

        string mSummary = "Summary";
        public string Summary
        {
            get { return mSummary; }
            set { SetProperty(ref mSummary, value); }
        }

        public MainPageViewModel(INavigation navigation,
                                  IDialogService dialogService) : base(navigation, dialogService)
        {

        }

        ICommand getWeatherCommand;
        public ICommand GetWeatherCommand => getWeatherCommand 
                                                    ?? (getWeatherCommand 
                                                            = new Command(async () 
                                                                    => await ExecuteGetWeatherCommand()));

        async Task ExecuteGetWeatherCommand()
        {
            try
            {
                IsBusy = true;

                IWeatherService weatherService = new WeatherService();

                var weatherToday = await weatherService.GetCurrentWeatherAsync(CityName);

                if (!weatherToday.IsSuccess)
                    throw new Exception(weatherToday.Information);

                City = weatherToday.CurrentWeather.name;

                Tempreature = weatherToday.CurrentWeather.main.temp;

                Condition = weatherToday.CurrentWeather.weather[0].main;

                Summary = string.Format("Wind: {0} | {1} deg", weatherToday.CurrentWeather.wind.speed.ToString(), weatherToday.CurrentWeather.wind.deg.ToString());

                var conditionType = (WeatherConditionType) Enum.Parse(typeof(WeatherConditionType), Condition.Replace(" ", "_"));

                var weatherCondition = weatherService.GetWeatherCondition(conditionType);

                BackgroundColor = weatherCondition.BackgroundColor;
                TextColor = weatherCondition.TextColor;
            }
            catch(Exception ex)
            {
                await DialogService.ShowMessage(App.AppName, ex.Message, "Close");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
