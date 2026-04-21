using System;
namespace MyWeatherApp.Models
{
    public class GetCurrentWeatherResult : BaseResult
    {
        public WeatherModel CurrentWeather
        {
            get;
            set;
        }
    }
}
