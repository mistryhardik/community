using System;
using System.Collections.Generic;
using System.Net;

namespace Fictional.CommandResults
{
    public class BaseResult
    {
        public bool IsSuccess
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public IList<string> Errors
        {
            get;
            set;
        }

        public HttpStatusCode ResponseStatusCode
        {
            get;
            set;
        }

        public BaseResult()
        {
            Message = string.Empty;
            Errors = new List<string>();
            ResponseStatusCode = HttpStatusCode.OK;
        }
    }
}
