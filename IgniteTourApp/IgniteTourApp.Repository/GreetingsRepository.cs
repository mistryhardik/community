using System;

namespace IgniteTourApp.Repository
{
    public interface IGreetingsRepository
    {
        string GetGreetingsMessage(int timeOfDay);
    }

    public class GreetingsRepository : IGreetingsRepository
    {
        public string GetGreetingsMessage(int timeOfDay)
        {
            if (timeOfDay < 12)
                return "Good morning";

            if (timeOfDay >= 12 & timeOfDay < 16)
                return "Good afternoon";

            if (timeOfDay >= 16 & timeOfDay <= 23)
                return "Good evening";

            return "Good morning";
        }
    }
}
