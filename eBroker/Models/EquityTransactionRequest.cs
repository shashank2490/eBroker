using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Models
{
    public class EquityTransactionRequest
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public int EquityId { get; set; }
        public int UnitsToBuySell { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
