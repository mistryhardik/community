using System;
using System.Threading.Tasks;
using MyWeatherApp.Models;

namespace MyWeatherApp.Repository
{
    public interface IWeatherService
    {
        Task<GetCurrentWeatherResult> GetCurrentWeatherAsync(string cityName);

        WeatherCondition GetWeatherCondition(WeatherConditionType type);

        Task<GetForecastWeatherResult> GetFiveDayForecastAsync(string cityName);
    }
}
