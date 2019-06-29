using System;
using Fictional.Repository;
using Xunit;

namespace Fictional.XUnitTests
{
    public class GreetingsUnitTests
    {
        IGreetingRepository greetingsRepository;

        public GreetingsUnitTests()
        {
            if (greetingsRepository == null)
                greetingsRepository = new GreetingRepository();
        }

        [Fact]
        public void TimeLessThan12ShouldReturnGoodMorning()
        {
            var message = greetingsRepository.GetGreetingMessage(11);

            Assert.NotNull(message);

            Assert.Equal("Good morning", message);
        }

        [Fact]
        public void TimeGreaterThan12ShouldReturnGoodAfternoon()
        {
            var message = greetingsRepository.GetGreetingMessage(13);

            Assert.NotNull(message);

            Assert.Equal("Good afternoon", message);
        }

        [Fact]
        public void TimeLessThan16ShouldReturnGoodAfternoon()
        {
            var message = greetingsRepository.GetGreetingMessage(15);

            Assert.NotNull(message);

            Assert.Equal("Good afternoon", message);
        }

        [Fact]
        public void TimeEqualTo16ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingMessage(16);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeGreaterThan16ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingMessage(18);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeLessThan23ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingMessage(22);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }

        [Fact]
        public void TimeEqualTo23ShouldReturnGoodEvening()
        {
            var message = greetingsRepository.GetGreetingMessage(23);

            Assert.NotNull(message);

            Assert.Equal("Good evening", message);
        }
    }
}
