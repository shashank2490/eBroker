using eBroker.Contracts;
using eBroker.Controllers;
using eBroker.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace eBroker_Test
{
    public class EquityControllerTest
    {
        EquityController _equityController;
        public EquityControllerTest()
        {
        }

        [Fact]
        public async Task BuyEquity_SuccessfullAsync()
        {
            Mock<IEquityService> equityServiceMock = new Mock<IEquityService>();
            _equityController = new EquityController(equityServiceMock.Object);

            var transactionId = 1;
            equityServiceMock.Setup(x => x.BuyEquityAsync(It.IsAny<EquityTransactionRequest>())).Returns(Task.FromResult(transactionId));

            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                AccountId = 1,
                UserId = 1,
                EquityId = 1,
                PricePerUnit = 20,
                UnitsToBuySell = 4
            };

            IActionResult result = await _equityController.BuyEquityAsync(equityTransactionRequest);

            var okResult = result as OkObjectResult;

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(transactionId, okResult.Value);
        }

        [Fact]
        public async Task BuyEquity_FailedAsync()
        {
            Mock<IEquityService> equityServiceMock = new Mock<IEquityService>();
            _equityController = new EquityController(equityServiceMock.Object);

            equityServiceMock.Setup(x => x.BuyEquityAsync(It.IsAny<EquityTransactionRequest>())).Throws<Exception>();

            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                AccountId = 1,
                UserId = 1,
                EquityId = 1,
                PricePerUnit = 20,
                UnitsToBuySell = 4
            };

            IActionResult result = await _equityController.BuyEquityAsync(equityTransactionRequest);

            var exceptionResult = result as ObjectResult;

            Assert.IsType<ObjectResult>(result);

            var expectedType = typeof(string);
            Assert.Equal(500, exceptionResult.StatusCode);
            Assert.IsType(expectedType, exceptionResult.Value);
        }

        [Fact]
        public async Task SellEquity_SuccessfullAsync()
        {
            Mock<IEquityService> equityServiceMock = new Mock<IEquityService>();
            _equityController = new EquityController(equityServiceMock.Object);

            var transactionId = 1;
            equityServiceMock.Setup(x => x.SellEquityAsync(It.IsAny<EquityTransactionRequest>())).Returns(Task.FromResult(transactionId));

            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                AccountId = 1,
                UserId = 1,
                EquityId = 1,
                PricePerUnit = 20,
                UnitsToBuySell = 4
            };

            IActionResult result = await _equityController.SellEquityAsync(equityTransactionRequest);

            var okResult = result as OkObjectResult;

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(transactionId, okResult.Value);
        }

        [Fact]
        public async Task SellEquity_FailedAsync()
        {
            Mock<IEquityService> equityServiceMock = new Mock<IEquityService>();
            _equityController = new EquityController(equityServiceMock.Object);

            equityServiceMock.Setup(x => x.SellEquityAsync(It.IsAny<EquityTransactionRequest>())).Throws<Exception>();

            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                AccountId = 1,
                UserId = 1,
                EquityId = 1,
                PricePerUnit = 20,
                UnitsToBuySell = 4
            };

            IActionResult result = await _equityController.SellEquityAsync(equityTransactionRequest);

            var exceptionResult = result as ObjectResult;

            Assert.IsType<ObjectResult>(result);

            var expectedType = typeof(string);
            Assert.Equal(500, exceptionResult.StatusCode);
            Assert.IsType(expectedType, exceptionResult.Value);
        }

    }
}
