using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class EquityTransaction: Item
    {
        public int CustomerId { get; set; }
        public int EquityId { get; set; }
        public int Units { get; set; }
        public decimal PricePerUnit { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
