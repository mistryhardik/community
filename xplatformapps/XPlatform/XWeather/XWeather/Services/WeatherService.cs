using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using XWeather.iOS.Models;

namespace XWeather.iOS.Services
{
    public class WeatherService
    {
        static string APP_ID = "70e46e9f04850eb081dda4fd1adef974";

        static string BASE_URL = "http://api.openweathermap.org/data/2.5/forecast/daily";

        public static string GetWeatherIcon(string iconCode)
        {
            return string.Format("http://openweathermap.org/img/w/{0}.png", iconCode);
        }

        public static async Task<WeatherResponse> GetWeather(string cityName)
        {
            try
            {
                var uriString = string.Format("{0}?q={1}&mode={2}&cnt={3}&units={4}&appid={5}", BASE_URL, cityName, WeatherResponseFormat.JSON, 7, WeatherUnit.Metric, APP_ID);

                HttpClient client = new HttpClient();

                var response = await client.GetAsync(new Uri(uriString));

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<WeatherResponse>(data);

                    Debug.WriteLine("Weather Service", "Result: " + result);
                    return result;
                }

                Debug.WriteLine("Weather Service", "Response failed with statuscode: " + response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Weather Service", ex.Message);
                return null;
            }
        }

        public static double kelvinToCelius(double temp)
        {
            return temp - 273.15;
        }

        public static double celsiusToFarenheit(double temp)
        {
            return (temp * 1.8) + 32;
        }
    }
}
