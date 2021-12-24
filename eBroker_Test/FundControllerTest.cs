using System;
using Xunit;
using eBroker.Controllers;
using Microsoft.AspNetCore.Mvc;
using eBroker.Contracts;
using Moq;
using eBroker.Models;
using System.Threading.Tasks;

namespace eBroker_Test
{
    public class FundControllerTest
    {
        FundsController _fundsController;
        public FundControllerTest()
        {
        }

        [Fact]
        public async Task AddFund_SuccessfullAsync()
        {
            Mock<IFundService> fundServiceMock = new Mock<IFundService>();
            _fundsController = new FundsController(fundServiceMock.Object);

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).Returns(Task.FromResult(true));

            AddFundRequest addFundRequest = new AddFundRequest
            {
                CustomerId = 1,
                Amount = 1000
            };

            IActionResult result = await _fundsController.AddFundsAsync(addFundRequest);

            var okResult = result as OkObjectResult;

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task AddFund_FailedAsync()
        {
            Mock<IFundService> fundServiceMock = new Mock<IFundService>();
            _fundsController = new FundsController(fundServiceMock.Object);

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).Returns(Task.FromResult(false));

            AddFundRequest addFundRequest = new AddFundRequest
            {
                CustomerId = 1,
                Amount = 1000
            };

            IActionResult result = await _fundsController.AddFundsAsync(addFundRequest);

            var okResult = result as OkObjectResult;

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public async Task AddFund_ExceptionAsync()
        {
            Mock<IFundService> fundServiceMock = new Mock<IFundService>();
            _fundsController = new FundsController(fundServiceMock.Object);

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).Throws(new Exception("Some error"));

            AddFundRequest addFundRequest = new AddFundRequest
            {
                CustomerId = 1,
                Amount = 1000
            };

            IActionResult result = await _fundsController.AddFundsAsync(addFundRequest);

            var exceptionResult = result as ObjectResult;

            Assert.IsType<ObjectResult>(result);
            var expectedType = typeof(string);
            Assert.Equal(500, exceptionResult.StatusCode);
            Assert.IsType(expectedType, exceptionResult.Value);
        }
    }
}
