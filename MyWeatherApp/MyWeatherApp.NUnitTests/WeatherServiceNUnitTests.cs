using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MyWeatherApp.NUnitTests
{
    [TestFixture]
    public class WeatherServiceNUnitTests : BaseNUnitTests
    {
        [SetUp]
        public void Setup()
        {
            SetupWeatherService();
        }

        [TestCase]
        public async Task As_A_User_I_Should_Be_Able_To_Get_Current_Weather()
        {
            var result = await weatherService.GetCurrentWeatherAsync("Vadodara");

            Assert.NotNull(result);

            Assert.AreEqual(result.IsSuccess, true);
        }
    }
}
