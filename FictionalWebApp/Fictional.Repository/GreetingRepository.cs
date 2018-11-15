using System;
namespace Fictional.Repository
{
    public class GreetingRepository : IGreetingRepository
    {
        public string GetGreetingMessage(int hourOfDay)
        {
            if(hourOfDay < 12)
            {
                return "Good morning";
            }
            else if (hourOfDay < 16)
            {
                return "Good afternoon";
            }
            else
            {
                return "Good evening";
            }
        }
    }
}
