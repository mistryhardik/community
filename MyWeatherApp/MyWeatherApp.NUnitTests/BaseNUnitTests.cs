using System;
using MyWeatherApp.Repository;

namespace MyWeatherApp.NUnitTests
{
    public class BaseNUnitTests
    {
        internal IWeatherService weatherService;

        public void SetupWeatherService()
        {
            if (weatherService == null)
                weatherService = new WeatherService();
        }
    }
}
