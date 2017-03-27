// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XWeather.iOS
{
    [Register ("DetailViewController")]
    partial class DetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView DetailRootView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ForecastDetailLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DetailRootView != null) {
                DetailRootView.Dispose ();
                DetailRootView = null;
            }

            if (ForecastDetailLabel != null) {
                ForecastDetailLabel.Dispose ();
                ForecastDetailLabel = null;
            }
        }
    }
}