using System;
using System.Net;

namespace MyWeatherApp.Models
{
    public class BaseResult
    {
        public bool IsSuccess
        {
            get;
            set;
        }

        public HttpStatusCode ResponseStatusCode
        {
            get;
            set;
        }

        public string Information
        {
            get;
            set;
        }
    }
}
