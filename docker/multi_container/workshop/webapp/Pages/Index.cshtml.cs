using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace webapp.Pages
{
    public class IndexModel : PageModel
    {
        public List<string> NewsFeed { get; set; }
        public async Task OnGetAsync()
        {
            HttpClient client = new HttpClient();
            // string json = await client.GetStringAsync("http:/localhost:1988/api/news");
            // string json = await client.GetStringAsync("http://172.17.0.3/api/news");
            string json = await client.GetStringAsync("http://newsfeed/api/news");
            NewsFeed = JsonConvert.DeserializeObject<List<string>>(json);
        }
    }
}
