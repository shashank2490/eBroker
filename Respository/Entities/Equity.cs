using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Equity: Item
    {
        public string EquityName { get; set; }
        public string EquityDescription { get; set; }
    }
}
