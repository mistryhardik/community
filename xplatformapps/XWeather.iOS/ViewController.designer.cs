// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace XWeather.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField CityNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CurrentCityLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView CurrentWeatherIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CurrentWeatherLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ForecastTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GetWeatherButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MainRootView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CityNameTextField != null) {
                CityNameTextField.Dispose ();
                CityNameTextField = null;
            }

            if (CurrentCityLabel != null) {
                CurrentCityLabel.Dispose ();
                CurrentCityLabel = null;
            }

            if (CurrentWeatherIcon != null) {
                CurrentWeatherIcon.Dispose ();
                CurrentWeatherIcon = null;
            }

            if (CurrentWeatherLabel != null) {
                CurrentWeatherLabel.Dispose ();
                CurrentWeatherLabel = null;
            }

            if (ForecastTableView != null) {
                ForecastTableView.Dispose ();
                ForecastTableView = null;
            }

            if (GetWeatherButton != null) {
                GetWeatherButton.Dispose ();
                GetWeatherButton = null;
            }

            if (MainRootView != null) {
                MainRootView.Dispose ();
                MainRootView = null;
            }
        }
    }
}