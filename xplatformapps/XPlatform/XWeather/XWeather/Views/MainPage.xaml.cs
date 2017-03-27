using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;
using XWeather.iOS.Services;

namespace XWeather.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GetWeatherButton_Clicked(object sender, EventArgs e)
        {
            // get weather forecast
            var weatherResponse = await WeatherService.GetWeather(CityNameEntry.Text);

            if (weatherResponse != null)
            {
                // get weather forecast
                var weatherForecast = weatherResponse.list[0];

                // get weather deatils
                var weatherDetail = weatherForecast.weather[0];

                // get what kind of weather is today
                var weatherDescription = weatherDetail.main;

                switch (weatherDescription)
                {
                    case "Clear":
                        {
                            // sky blue
                            RootLayout.BackgroundColor = Color.FromRgb(52, 152, 219);
                        }
                        break;
                    case "Clouds":
                        {
                            // gray
                            RootLayout.BackgroundColor = Color.FromRgb(149, 165, 166);
                        }
                        break;
                    default:
                        {
                            // something random
                            RootLayout.BackgroundColor = Color.FromRgb(26, 188, 156);
                        }
                        break;
                }

                // set the text values
                CurrentCityLabel.Text = weatherResponse.city.name;
                CurrentWeatherLabel.Text = weatherForecast.temp.max.ToString();

                // get icon url
                var weatherIconUrl = WeatherService.GetWeatherIcon(weatherDetail.icon);

                // set the image from url as source of Image
                CurrentWeatherIcon.Source = ImageSource.FromUri(new Uri(weatherIconUrl));

                // format the forecast details as a string
                var forecastItems = beautifyForecast(weatherResponse.list);

                // set up the table source for the table view
                ForecastListView.ItemsSource = forecastItems;
            }
            else
            {
                // just say we couldn't get the work done
                await DisplayAlert("Message", "Weather details not found for: " + CityNameEntry.Text, "Close");
            }
        }

        // format the weather forcast into a single string in format as: Date - Min - Max
        private string[] beautifyForecast(List<iOS.Models.List> forecastItems)
        {
            try
            {
                string[] items;

                if (forecastItems == null)
                    return null;

                if (forecastItems.Count > 0)
                {
                    items = new string[forecastItems.Count];

                    for (int i = 0; i < forecastItems.Count; i++)
                    {
                        var forecastItemToShow = forecastItems[i];

                        var dateToShow = UnixTimeStampToDateTime(forecastItemToShow.dt);

                        items[i] = string.Format("{0} - {1} - {2}", dateToShow.ToString("dd/MM/yyyy"), forecastItemToShow.temp.min, forecastItemToShow.temp.max);
                    }

                    return items;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("MainPage : beautifyForecast", ex.Message);

                return null;
            }
        }

        // reference: https://coderwall.com/p/e8rzuq/how-to-convert-a-unix-timestamp-to-a-net-system-datetime-object
        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Format our new DateTime object to start at the UNIX Epoch
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the timestamp (number of seconds since the Epoch) to be converted
            dateTime = dateTime.AddSeconds(unixTimeStamp);

            return dateTime;
        }
    }
}
