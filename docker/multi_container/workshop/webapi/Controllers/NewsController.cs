using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed.Rss;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            HttpClient client = new HttpClient();
            string rss = await client.GetStringAsync("https://devblogs.microsoft.com/devops/feed/");

            List<string> news = new List<string>();
            using (var xmlReader = XmlReader.Create(new StringReader(rss), new XmlReaderSettings { Async = true }))
            {
                RssFeedReader feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (feedReader.ElementType == Microsoft.SyndicationFeed.SyndicationElementType.Item)
                    {
                        var item = await feedReader.ReadItem();
                        news.Add(item.Title);
                    }
                }
            }

            return news;
        }
    }
}
