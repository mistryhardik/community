using System;
using Fictional.Repository;
using NUnit.Framework;

namespace Fictional.NUnitTests
{
    [TestFixture]
    public class FictionalNUnitTests
    {
        IGreetingRepository greetingRepository;

        [SetUp]
        public void Setup()
        {
            greetingRepository = new GreetingRepository();
        }

        [TestCase]
        public void Should_Return_Good_Morning_For_Hour_Greater_Than_12()
        {
            var fictionalTime = new DateTime(2018, 09, 16, 09, 30, 00);

            var greetingResult = greetingRepository.GetGreetingMessage(fictionalTime.Hour);

            var expectedResult = "Good morning";

            Assert.AreEqual(expectedResult, greetingResult);
        }

        [TestCase]
        public void Should_Return_Good_Afternoon_For_Hour_Greater_Than_16()
        {
            var fictionalTime = new DateTime(2018, 09, 16, 14, 30, 00);

            var greetingResult = greetingRepository.GetGreetingMessage(fictionalTime.Hour);

            var expectedResult = "Good afternoon";

            Assert.AreEqual(expectedResult, greetingResult);
        }

        [TestCase]
        public void Should_Return_Good_Evening_For_Hour_Greater_Than_X()
        {
            var fictionalTime = new DateTime(2018, 09, 16, 18, 30, 00);

            var greetingResult = greetingRepository.GetGreetingMessage(fictionalTime.Hour);

            var expectedResult = "Good evening";

            Assert.AreEqual(expectedResult, greetingResult);
        }
    }
}
