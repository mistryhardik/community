using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using MyWeatherApp.Models;
using Newtonsoft.Json;

namespace MyWeatherApp.Repository
{
    public class WeatherService : IWeatherService
    {
        string OpenWeatherMapApiKey = "70e46e9f04850eb081dda4fd1adef974";
        string OpenWeatherMapCurrentWeatherApi = "https://api.openweathermap.org/data/2.5/weather";
        string OpenWeatherMapFiveDayForecastApi = "https://api.openweathermap.org/data/2.5/forecast/daily";

        public async Task<GetCurrentWeatherResult> GetCurrentWeatherAsync(string cityName)
        {
            try
            {
                if (string.IsNullOrEmpty(cityName))
                    throw new Exception("City name is required");

                HttpClient client = new HttpClient();

                var requestUrl = string.Format("{0}?q={1}&appid={2}&units=metric", OpenWeatherMapCurrentWeatherApi, cityName, OpenWeatherMapApiKey);

                var result = await client.GetAsync(requestUrl);

                if (!result.IsSuccessStatusCode)
                    throw new Exception("Error occured: " + result.ReasonPhrase);

                var content = await result.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WeatherModel>(content);

                return new GetCurrentWeatherResult
                {
                    IsSuccess = true,
                    CurrentWeather = response,
                    Information = "It's " + response.weather[0].main + " today in " + cityName,
                    ResponseStatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new GetCurrentWeatherResult
                {
                    ResponseStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Information = ex.Message
                };
            }
        }

        public WeatherCondition GetWeatherCondition(WeatherConditionType type)
        {
            try
            {
                switch(type)
                {
                    case WeatherConditionType.Thunderstrom:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.DarkGray,
                                TextColor = Color.White,
                                Type = WeatherConditionType.Thunderstrom
                            };
                        }
                    case WeatherConditionType.Drizzle:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.LightGray,
                                TextColor = Color.White,
                                Type = WeatherConditionType.Drizzle
                            };
                        }
                    case WeatherConditionType.Rain:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.PowderBlue,
                                TextColor = Color.Black,
                                Type = WeatherConditionType.Rain
                            };
                        }
                    case WeatherConditionType.Snow:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.White,
                                TextColor = Color.DarkGray,
                                Type = WeatherConditionType.Snow
                            };
                        }
                    case WeatherConditionType.Mist:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.GhostWhite,
                                TextColor = Color.DarkGray,
                                Type = WeatherConditionType.Snow
                            };
                        }
                    case WeatherConditionType.Smoke:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.SlateGray,
                                TextColor = Color.Gray,
                                Type = WeatherConditionType.Smoke
                            };
                        }
                    case WeatherConditionType.Haze:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.OrangeRed,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Haze
                            };
                        }
                    case WeatherConditionType.Sand:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.SandyBrown,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Sand
                            };
                        }
                    case WeatherConditionType.Dust_Whrils:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.RosyBrown,
                                TextColor = Color.Black,
                                Type = WeatherConditionType.Dust_Whrils
                            };
                        }
                    case WeatherConditionType.Fog:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.LightSlateGray,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Fog
                            };
                        }
                    case WeatherConditionType.Dust:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.RosyBrown,
                                TextColor = Color.Black,
                                Type = WeatherConditionType.Dust_Whrils
                            };
                        }
                    case WeatherConditionType.Volcanic_Ash:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.OrangeRed,
                                TextColor = Color.DarkGray,
                                Type = WeatherConditionType.Volcanic_Ash
                            };
                        }
                    case WeatherConditionType.Squalls:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.LightBlue,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Squalls
                            };
                        }
                    case WeatherConditionType.Tornado:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.PaleVioletRed,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Tornado
                            };
                        }
                    case WeatherConditionType.Clear:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.SkyBlue,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Clear
                            };
                        }
                    case WeatherConditionType.Clouds:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.WhiteSmoke,
                                TextColor = Color.SlateGray,
                                Type = WeatherConditionType.Clouds
                            };
                        }
                    default:
                        {
                            return new WeatherCondition
                            {
                                BackgroundColor = Color.SkyBlue,
                                TextColor = Color.Navy,
                                Type = WeatherConditionType.Clear
                            };
                        }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetForecastWeatherResult> GetFiveDayForecastAsync(string cityName)
        {
            try
            {
                if (string.IsNullOrEmpty(cityName))
                    throw new Exception("City name is required");

                HttpClient client = new HttpClient();

                var requestUrl = string.Format("{0}?q={1}&appid={2}&units=metric&cnt=5", OpenWeatherMapFiveDayForecastApi, cityName, OpenWeatherMapApiKey);

                var result = await client.GetAsync(requestUrl);

                if (!result.IsSuccessStatusCode)
                    throw new Exception("Error occured: " + result.ReasonPhrase);

                var content = await result.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WeatherForecastModel>(content);

                return new GetForecastWeatherResult
                {
                    IsSuccess = true,
                    Forecast = response,
                    ResponseStatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new GetForecastWeatherResult
                {
                    ResponseStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Information = ex.Message
                };
            }
        }
    }
}
