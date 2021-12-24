using eBroker.Contracts;
using eBroker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eBroker_Test
{
    public class SystemClockTest
    {
        [Fact]
        public void NowTest()
        {
            ISystemClock systemClock = new SystemClock();
            DateTime systemNow = systemClock.GetCurrentTime();

            Assert.InRange(systemNow.Second, DateTime.Now.Second - 1, DateTime.Now.Second);
        } 
    }
}
