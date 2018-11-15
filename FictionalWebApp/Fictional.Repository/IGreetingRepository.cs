using System;
namespace Fictional.Repository
{
    public interface IGreetingRepository
    {
        string GetGreetingMessage(int hourOfDay);
    }
}
