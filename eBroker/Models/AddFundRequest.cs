using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Models
{
    public class AddFundRequest
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
