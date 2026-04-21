using System;
namespace MyWeatherApp.Models
{
    public class GetForecastWeatherResult : BaseResult
    {
        public WeatherForecastModel Forecast
        {
            get;
            set;
        }
    }
}
