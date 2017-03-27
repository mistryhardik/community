using Foundation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using XWeather.iOS.Services;

namespace XWeather.iOS
{
    public partial class ViewController : UIViewController
    {
        LoadingOverlay loadingOverlay;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            GetWeatherButton.TouchUpInside += GetWeatherButton_TouchUpInside;
        }

        private async void GetWeatherButton_TouchUpInside(object sender, EventArgs e)
        {
            // calculate x, y position of the screen
            var bounds = UIScreen.MainScreen.Bounds;

            // initialize object instance of LoadingOverlay class
            loadingOverlay = new LoadingOverlay(bounds);

            // add the progress indicator to view
            View.Add(loadingOverlay);

            // get weather forecast
            var weatherResponse = await WeatherService.GetWeather(CityNameTextField.Text);

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
                            MainRootView.BackgroundColor = UIColor.FromRGB(52, 152, 219);
                        }
                        break;
                    case "Clouds":
                        {
                            // gray
                            MainRootView.BackgroundColor = UIColor.FromRGB(149, 165, 166);
                        }
                        break;
                    default:
                        {
                            // something random
                            MainRootView.BackgroundColor = UIColor.FromRGB(26, 188, 156);
                        }
                        break;
                }

                // set the text values
                CurrentCityLabel.Text = weatherResponse.city.name;
                CurrentWeatherLabel.Text = weatherForecast.temp.max.ToString();

                // get icon url
                var weatherIconUrl = WeatherService.GetWeatherIcon(weatherDetail.icon);

                // get UIIMage from Url
                var uiImage = UIImageFromUrl(weatherIconUrl);

                // set the UIImage to ImageView
                CurrentWeatherIcon.Image = uiImage;

                // format the forecast details as a string
                var forecastItems = beautifyForecast(weatherResponse.list);

                // set up the table source for the table view
                ForecastTableView.Source = new TableSource(forecastItems, this);

                // refresh the table view
                ForecastTableView.ReloadData();
            }
            else
            {
                // just say we couldn't get the work done
                new UIAlertView("Message", "Weather details not found for: " + CityNameTextField.Text, null, "Close", null).Show();
            }

            // hide the progress indicator
            loadingOverlay.Hide();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        // method to handle OnItemTouch event in TableSource
        public void ShowForecastDetail(string foreCastItem)
        {
            // get instance of the ViewController (In this case the ID is DetailViewController, it may differ for your example)
            var detailViewController = this.Storyboard.InstantiateViewController("DetailViewController") as DetailViewController;

            if (detailViewController != null)
            {
                // set the item value
                detailViewController.forecastDetail = foreCastItem;

                // navigate to new ViewController
                this.NavigationController.PushViewController(detailViewController, true);
            }
        }

        // format the weather forcast into a single string in format as: Date - Min - Max
        private string[] beautifyForecast(List<Models.List> forecastItems)
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

                        items[i] = string.Format("{0} - {1} - {2}", dateToShow.ToShortDateString(), forecastItemToShow.temp.min, forecastItemToShow.temp.max);
                    }

                    return items;
                }

                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("MainActivity : beautifyForecast", ex.Message);

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

        // reference: https://alexdunn.org/2015/06/03/creating-a-uiimage-with-a-url-in-xamarin-ios/
        public UIImage UIImageFromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            {
                using (var data = NSData.FromUrl(url))
                    return UIImage.LoadFromData(data);
            }
        }
    }
}