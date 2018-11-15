using System;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyWeatherApp.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class WeatherUITests : BaseTests
    {
        public WeatherUITests(Platform platform) : base(platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            BaseTestsBeforeEachTest();
        }

        [Test]
        public void Get_Current_Weather()
        {
            app.Screenshot("Home page");

            switch(platform)
            {
                case Platform.Android:
                    {
                        app.EnterText(e => e.Class("FormsEditText").Marked("City"), City);

                        app.DismissKeyboard();

                        app.Tap(b => b.Marked("@"));

                        Thread.Sleep(TimeSpan.FromSeconds(FiveSecondsTimeout));

                        app.Screenshot("Weather for " + City);

                        var queryResult = app.Query(l => l.Text(City));

                        Assert.GreaterOrEqual(queryResult.Length, 1);

                        break;
                    }
                case Platform.iOS:
                    {
                        app.EnterText(e => e.Class("UITextField").Marked("City"), City);

                        app.Tap(b => b.Marked("@"));

                        Thread.Sleep(TimeSpan.FromSeconds(FiveSecondsTimeout));

                        app.Screenshot("Weather for " + City);

                        var queryResult = app.Query(l => l.Text(City));

                        Assert.GreaterOrEqual(queryResult.Length, 1);

                        break;
                    }
            }
        }
    }
}
