using eBroker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Services
{
    public class SystemClock : ISystemClock
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}
