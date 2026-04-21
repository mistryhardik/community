using System;
using System.Collections.Generic;
using Fictional.ViewModels;

namespace Fictional.CommandResults
{
    public class GetProductsResult : BaseResult
    {
        public IList<ProductViewModel> Products
        {
            get;
            set;
        }
    }
}
