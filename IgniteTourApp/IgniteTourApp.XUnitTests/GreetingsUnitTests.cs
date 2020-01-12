using System;
using IgniteTourApp.Repository;
using Xunit;

namespace IgniteTourApp.XUnitTests
{
    public class GreetingsUnitTests
    {
        IGreetingsRepository greetingsRepository;

        public GreetingsUnitTests()
        {
            if (greetingsRepository == null)
                greetingsRepository = new GreetingsRepository();
        }

        [Fact]
        public void TimeLessThan12ShouldReturnGoodMorning()
        {
            var message = greetingsRepository.GetGreetingsMessage(11);

            Assert.NotNull(message);

            Assert.Equal("Good morning", message);
        }

        [Fact]
        public void TimeGreaterThan12ShouldReturnGoodAfternoon()
        {
            var message = greetingsRepository.GetGreetingsMessage(13);

            Assert.NotNull(message);

            Assert.Equal("Good afternoon", message); ;
        }

        [Fact]
        public void TimeLessThan16ShouldReturnGoodAfternoon()
        {
            var message = greetingsRepository.GetGreetingsMessage(15);

            Assert.NotNull(message);

            Assert.Equal("Good afternoon", message);
        }

        [Fact]
        public void TimeEqualTo16ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingsMessage(16);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeGreaterThan16ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingsMessage(18);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeLessThan23ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingsMessage(22);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeEqualTo23ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingsMessage(23);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }
    }
}
