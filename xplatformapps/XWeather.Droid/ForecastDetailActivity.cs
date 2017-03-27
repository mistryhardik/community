using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XWeather.Droid
{
    [Activity(Label = "Forecast Detail")]
    public class ForecastDetailActivity : Activity
    {
        static string KEY_BUNDLE_FORECAST = "FORECAST_BUNDLE";

        TextView forecastDate, forecastMinTemperature, forecastMaxTemperature;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ForecastDetailActivity);

            forecastDate = FindViewById<TextView>(Resource.Id.ForecastDateTextView);
            forecastMinTemperature = FindViewById<TextView>(Resource.Id.ForecastMinTemperature);
            forecastMaxTemperature = FindViewById<TextView>(Resource.Id.ForecastMaxTemperature);

            // get the forecast details as string which you sent from MainActivity
            string forecastBundle = Intent.GetStringExtra(KEY_BUNDLE_FORECAST);

            // split the string to seperate relavant information
            string[] forecastBundleArr = forecastBundle.Split(' ');

            // assign the values
            forecastDate.Text = forecastBundleArr[0];
            forecastMinTemperature.Text = forecastBundleArr[2];
            forecastMaxTemperature.Text = forecastBundleArr[4];
        }
    }
}