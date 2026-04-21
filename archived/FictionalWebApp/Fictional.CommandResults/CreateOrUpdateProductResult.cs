using System;
using Fictional.ViewModels;

namespace Fictional.CommandResults
{
    public class CreateOrUpdateProductResult : BaseResult
    {
        public ProductViewModel Product
        {
            get;
            set;
        }
    }
}
