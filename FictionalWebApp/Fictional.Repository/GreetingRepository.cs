using System;
namespace Fictional.Repository
{
    public class GreetingRepository : IGreetingRepository
    {
        public string GetGreetingMessage(int hourOfDay)
        {
            if (hourOfDay < 12)
                return "Good morning";

            if (hourOfDay >= 12 & hourOfDay < 16)
                return "Good afternoon";

            if (hourOfDay >= 16 & hourOfDay <= 23)
                return "Good evening";

            return "Good afternoon";
        }
    }
}
