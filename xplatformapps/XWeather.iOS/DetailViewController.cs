using Foundation;
using System;
using UIKit;

namespace XWeather.iOS
{
    public partial class DetailViewController : UIViewController
    {
        public string forecastDetail;

        public DetailViewController (IntPtr handle) : base (handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // explicitly setting the title for the page
            Title = "Detail";

            // setting the forecast value
            ForecastDetailLabel.Text = forecastDetail;

            // logging the forecast value, just like that ;)
            Console.WriteLine("Forecast detail: " + forecastDetail);
        }
    }
}