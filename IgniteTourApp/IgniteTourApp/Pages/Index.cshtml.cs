using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IgniteTourApp.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IgniteTourApp.Pages
{
    public class IndexModel : PageModel
    {
        public string Message;
        IGreetingsRepository greetingsRepository;

        public IndexModel()
        {
            if (greetingsRepository == null)
                greetingsRepository = new GreetingsRepository();
        }

        public void OnGet()
        {
            Message = greetingsRepository.GetGreetingsMessage(DateTime.Now.Hour);
        }
    }
}
