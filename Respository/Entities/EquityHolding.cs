using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class EquityHolding: Item
    {
        public int CustomerId { get; set; }
        
        public int EquityId { get; set; }
        public int Units { get; set; }
        [ForeignKey("EquityId")]
        public virtual Equity Equity { get; set; }
    }
}
