using System;
using Xamarin.UITest;

namespace MyWeatherApp.UITests
{
    public class BaseTests
    {
        internal IApp app;
        internal Platform platform;

        internal int FiveSecondsTimeout = 5;
        internal int TenSecondsTimeout = 10;
        internal int FifteenSecondsTimeout = 15;

        internal string City = "Vadodara";

        public BaseTests(Platform platform)
        {
            this.platform = platform;
        }

        public void BaseTestsBeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }
    }
}
