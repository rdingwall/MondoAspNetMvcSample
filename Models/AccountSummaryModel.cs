using System.Collections.Generic;
using Mondo;

namespace MondoAspNetMvcSample.Models
{
    public sealed class AccountSummaryModel
    {
        public Account Account { get; set; }

        public IList<Transaction> Transactions { get; set; }

        public Balance Balance { get; set; }
    }
}