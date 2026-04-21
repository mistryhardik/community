using System;
using Fictional.ViewModels;

namespace Fictional.CommandResults
{
	public class GetProductResult : BaseResult
    {
        public ProductViewModel Product
        {
            get;
            set;
        }
    }
}
