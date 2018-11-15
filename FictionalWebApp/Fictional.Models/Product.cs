using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Fictional.Models
{
    public class Product : TableEntity
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public string RedirectUrl
        {
            get;
            set;
        }
    }
}
