using eBroker;
using eBroker.Contracts;
using eBroker.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eBroker_Test
{
    public class StartupTests
    {
        [Fact]
        public void StartupTest()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.NotNull(webHost);
            Assert.NotNull(webHost.Services.GetService(typeof(IEquityService)));
            Assert.NotNull(webHost.Services.GetService(typeof(IFundService)));
        }
    }
}
