using Android.App;
using Android.Widget;
using Android.OS;
using XWeather.Droid.Services;
using Android.Net;
using Android.Util;
using System.Collections.Generic;
using System;
using Android.Content;
using Android.Graphics;
using System.Threading.Tasks;
using System.Net.Http;

namespace XWeather.Droid
{
    [Activity(Label = "XWeather", MainLauncher = true, Icon = "@mipmap/ic_launcher", WindowSoftInputMode = Android.Views.SoftInput.StateAlwaysHidden)]
    public class MainActivity : Activity
    {
        static string KEY_BUNDLE_FORECAST = "FORECAST_BUNDLE";

        EditText cityEditText;

        Button getWeatherButton;
        
        TextView currentCityTextView, currentMaxTemperatureTextView;

        ImageView weatherIcon;
        
        ListView forecastListView;

        LinearLayout rootLayout;

        GridLayout weatherDetailsGridLayout;

        ProgressDialog progressDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);

            cityEditText = FindViewById<EditText>(Resource.Id.CityEditText);

            getWeatherButton = FindViewById<Button>(Resource.Id.GetWeatherButton);

            currentCityTextView = FindViewById<TextView>(Resource.Id.CityNameTextView);

            currentMaxTemperatureTextView = FindViewById<TextView>(Resource.Id.MaxTemperatureTextView);

            forecastListView = FindViewById<ListView>(Resource.Id.ForecastListView);

            weatherIcon = FindViewById<ImageView>(Resource.Id.WeatherImageView);

            rootLayout = FindViewById<LinearLayout>(Resource.Id.rootLayout);

            weatherDetailsGridLayout = FindViewById<GridLayout>(Resource.Id.gridLayout1);

            getWeatherButton.Click += GetWeather_Click;
            forecastListView.ItemClick += ForecastListView_ItemClick;
        }

        private void ForecastListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = forecastListView.GetItemAtPosition(e.Position);

            Toast.MakeText(this, item.ToString(), ToastLength.Short).Show();

            // create an intent to contain an activity
            var forecastDetailActivity = new Intent(this, typeof(ForecastDetailActivity));

            // add some information to pass to the new activity
            forecastDetailActivity.PutExtra(KEY_BUNDLE_FORECAST, item.ToString());

            // start the activity
            StartActivity(forecastDetailActivity);
        }

        private async void GetWeather_Click(object sender, System.EventArgs e)
        {
            // initialize and show a progress bar
            progressDialog = new ProgressDialog(this);
            progressDialog.Indeterminate = true;
            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            progressDialog.SetMessage("Please wait...");
            progressDialog.SetCancelable(false);
            progressDialog.Show();

            // check if the network is available
            if (isConnectedToNetwork())
            {
                // request for weather in the city
                var weatherResponse = await WeatherService.GetWeather(cityEditText.Text);

                // if not null, fill up the UI
                if (weatherResponse != null)
                {
                    // get weather forecast
                    var weatherForecast = weatherResponse.list[0];

                    // get weather deatils
                    var weatherDetail = weatherForecast.weather[0];

                    if (weatherDetailsGridLayout.Visibility == Android.Views.ViewStates.Gone)
                        weatherDetailsGridLayout.Visibility = Android.Views.ViewStates.Visible;

                    // get what kind of weather is today
                    var weatherDescription = weatherDetail.main;

                    switch(weatherDescription)
                    {
                        case "Clear":
                            {
                                rootLayout.SetBackgroundColor(Color.LightBlue);
                            }
                            break;
                        case "Clouds":
                            {
                                rootLayout.SetBackgroundColor(Color.LightGray);
                            }
                            break;
                        default:
                            {
                                rootLayout.SetBackgroundColor(Color.LightCoral);
                            }
                            break;
                    }

                    // set the details
                    currentCityTextView.Text = weatherResponse.city.name;
                    currentMaxTemperatureTextView.Text = string.Format("{0:F1}", weatherForecast.temp.max);

                    // get icon url
                    var weatherIconUrl = WeatherService.GetWeatherIcon(weatherDetail.icon);
                    
                    var imageBitmap = await GetImageBitmapFromUrlAsync(weatherIconUrl);

                    weatherIcon.SetImageBitmap(imageBitmap);

                    var forecastItems = beautifyForecast(weatherResponse.list);

                    forecastListView.Adapter = new ForecastListAdapter(this, forecastItems);
                }
                else
                {
                    // just say something didn't work
                    Toast.MakeText(this.ApplicationContext, "Unable to get weather information", ToastLength.Long).Show();
                }
            }
            else
            {
                // just say something didn't work
                Toast.MakeText(this.ApplicationContext, "Not connected to network", ToastLength.Long).Show();
            }

            // hide the progress bar
            progressDialog.Dismiss();
        }

        // explicit method to check if device is connected to network
        private bool isConnectedToNetwork()
        {
            try
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

                NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

                if (networkInfo != null)
                {
                    bool isOnline = networkInfo.IsConnected;

                    return isOnline;
                }

                return false;
            }
            catch (System.Exception ex)
            {
                Log.Error("MainActivity : isConnectedToNetwork", ex.Message);

                return false;
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
            catch(System.Exception ex)
            {
                Log.Error("MainActivity : beautifyForecast", ex.Message);

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

        private async Task<Bitmap> GetImageBitmapFromUrlAsync(string url)
        {
            Bitmap imageBitmap = null;

            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}

