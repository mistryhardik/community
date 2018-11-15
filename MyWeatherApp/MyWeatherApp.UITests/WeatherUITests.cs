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

            app.EnterText(e => e.Class("FormsEditText").Marked("City"), City);

            app.DismissKeyboard();

            app.Tap(b => b.Marked("@"));

            Thread.Sleep(TimeSpan.FromSeconds(TenSecondsTimeout));

            app.Screenshot("Weather for " + City);

            var queryResult = app.Query(l => l.Text(City));

            Assert.GreaterOrEqual(queryResult.Length, 1);
        }
    }
}
