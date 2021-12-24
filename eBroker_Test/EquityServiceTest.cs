using eBroker.Contracts;
using eBroker.Models;
using eBroker.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eBroker_Test
{
    public class EquityServiceTest
    {
        Mock<IFundService> fundServiceMock;
        Mock<IGenericRepository<EquityHolding>> mockEquityHoldingRepository;
        Mock<IGenericRepository<EquityTransaction>> mockEquityTransactionRepository;
        Mock<ISystemClock> mockSystemClock;

        public EquityServiceTest()
        {
            fundServiceMock = new Mock<IFundService>();
            mockEquityHoldingRepository = new Mock<IGenericRepository<EquityHolding>>();
            mockEquityTransactionRepository = new Mock<IGenericRepository<EquityTransaction>>();
            mockSystemClock = new Mock<ISystemClock>();
        }

        #region Buy Equity Unit Tests
        [Fact]
        public async Task EquityBuy_SuccessfullAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 3,
                AccountId = 3,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 3000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 20,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityHolding>()))
                .ReturnsAsync((EquityHolding equityHolding) => { return equityHolding; });

            mockEquityHoldingRepository.Setup(x => x.Save());


            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.BuyEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task EquityBuy_NoExitingEquityAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 2,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ReturnsAsync((AddFundRequest addFundRequest) =>
            {
                return true;
            });

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    return new List<EquityHolding>();
                });

            mockEquityHoldingRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityHolding>()))
                .ReturnsAsync((EquityHolding equityHolding) => { return equityHolding; });

            mockEquityHoldingRepository.Setup(x => x.Save());

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.BuyEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task EquityBuy_ExitingEquityAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ReturnsAsync((AddFundRequest addFundRequest) =>
            {
                return true;
            });

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                    {
                        var currentHoldings = new List<EquityHolding>();
                        currentHoldings.Add(new EquityHolding
                        {
                            Id = 1,
                            EquityId = 1,
                            Units = 20,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                        });

                        return currentHoldings;
                    });

            mockEquityHoldingRepository.Setup(x => x.Update(It.IsAny<EquityHolding>()));
            mockEquityHoldingRepository.Setup(x => x.Save());

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.BuyEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1,result);
        }

        [Fact]
        public async Task EquityBuy_BalanceInsufficientAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 100,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 12, 30, 30));

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.BuyEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Insufficient funds. Please add funds to your account to buy Equity.", ex.Message);
        }

        [Fact]
        public async Task EquityBuy_TransactionTimeBefore9AMAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 8, 30, 30));

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(()=> equityService.BuyEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.", ex.Message);
        }

        [Fact]
        public async Task EquityBuy_TransactionTimeAfter3PMAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 15, 1, 0));

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.BuyEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.", ex.Message);
        }

        [Fact]
        public async Task EquityBuy_UpdateFundThrowsException_NewEquityAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 2,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ThrowsAsync(new Exception("Some exception occurred"));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    return new List<EquityHolding>();
                });

            mockEquityHoldingRepository.Setup(x => x.Delete(It.IsAny<EquityHolding>()));

            mockEquityHoldingRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => equityService.BuyEquityAsync(equityTransactionRequest));

            Assert.IsType<Exception>(ex);
            Assert.Equal("Some exception occurred", ex.Message);
        }

        [Fact]
        public async Task EquityBuy_UpdateFundThrowsException_ExitingEquityAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ThrowsAsync(new Exception("Some exception occurred"));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 20,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository.Setup(x => x.Update(It.IsAny<EquityHolding>()));
            mockEquityHoldingRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => equityService.BuyEquityAsync(equityTransactionRequest));

            Assert.IsType<Exception>(ex);
            Assert.Equal("Some exception occurred", ex.Message);
        }

        #endregion

        #region Sell Equity Unit Tests
        [Fact]
        public async Task EquitySell_SuccessfullAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 3,
                AccountId = 3,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 3000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 100,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityHolding>()))
                .ReturnsAsync((EquityHolding equityHolding) => { return equityHolding; });

            mockEquityHoldingRepository.Setup(x => x.Save());

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.SellEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task EquitySell_20BrokerageAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 3,
                AccountId = 3,
                EquityId = 1,
                PricePerUnit = 1,
                UnitsToBuySell = 10
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 3000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 100,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityHolding>()))
                .ReturnsAsync((EquityHolding equityHolding) => { return equityHolding; });

            mockEquityHoldingRepository.Setup(x => x.Save());

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.SellEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task EquitySell_PercentBrokerageAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 3,
                AccountId = 3,
                EquityId = 1,
                PricePerUnit = 1000,
                UnitsToBuySell = 1000
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 3000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 10000,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityHolding>()))
                .ReturnsAsync((EquityHolding equityHolding) => { return equityHolding; });

            mockEquityHoldingRepository.Setup(x => x.Save());

            mockEquityTransactionRepository
                .Setup(x => x.InsertAsync(It.IsAny<EquityTransaction>()))
                .ReturnsAsync((EquityTransaction equityTransaction) =>
                {
                    return new EquityTransaction
                    {
                        Id = 1,
                        EquityId = equityTransactionRequest.EquityId,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = Repository.Enums.TransactionType.Buy,
                        Units = equityTransactionRequest.UnitsToBuySell
                    };
                });

            mockEquityTransactionRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var result = await equityService.SellEquityAsync(equityTransactionRequest);

            Assert.IsType<int>(result);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task EquitySell_NoExitingEquityAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 2,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            Mock<IFundService> fundServiceMock = new Mock<IFundService>();

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ReturnsAsync((AddFundRequest addFundRequest) =>
            {
                return true;
            });

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    return new List<EquityHolding>();
                });

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.SellEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("No Equity units in holding to sell.", ex.Message);
        }

        [Fact]
        public async Task EquitySell_ExitingEquityIsInsufficientAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 2,
                PricePerUnit = 100,
                UnitsToBuySell = 100
            };

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ReturnsAsync((AddFundRequest addFundRequest) =>
            {
                return true;
            });

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 10,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.SellEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Equity Units to sell are greater than Current Equity unit holding.", ex.Message);
        }

        [Fact]
        public async Task EquitySell_TransactionTimeBefore9AMAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 8, 30, 30));

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.SellEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.", ex.Message);
        }

        [Fact]
        public async Task EquitySell_TransactionTimeAfter3PMAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 15, 1, 0));

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => equityService.SellEquityAsync(equityTransactionRequest));

            Assert.IsType<InvalidOperationException>(ex);
            Assert.Equal("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.", ex.Message);
        }

        [Fact]
        public async Task EquitySell_UpdateFundThrowsExceptionAsync()
        {
            EquityTransactionRequest equityTransactionRequest = new EquityTransactionRequest
            {
                UserId = 1,
                AccountId = 1,
                EquityId = 1,
                PricePerUnit = 100,
                UnitsToBuySell = 10
            };

            mockSystemClock.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2021, 12, 23, 10, 30, 30));

            fundServiceMock.Setup(x => x.GetAccountAsync(It.IsAny<int>())).ReturnsAsync((int a) =>
            {
                return new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 10000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                };
            });

            fundServiceMock.Setup(x => x.AddFundsAsync(It.IsAny<AddFundRequest>())).ThrowsAsync(new Exception("Some exception occurred"));

            mockEquityHoldingRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<EquityHolding, bool>>>(), It.IsAny<Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<EquityHolding, bool>> filter, Func<IQueryable<EquityHolding>, IOrderedQueryable<EquityHolding>> orderBy, string includeProperties) =>
                {
                    var currentHoldings = new List<EquityHolding>();
                    currentHoldings.Add(new EquityHolding
                    {
                        Id = 1,
                        EquityId = 1,
                        Units = 20,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return currentHoldings;
                });

            mockEquityHoldingRepository.Setup(x => x.Update(It.IsAny<EquityHolding>()));

            mockEquityHoldingRepository.Setup(x => x.Save());

            IEquityService equityService = new EquityService(mockEquityTransactionRepository.Object, mockEquityHoldingRepository.Object, fundServiceMock.Object, mockSystemClock.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => equityService.SellEquityAsync(equityTransactionRequest));

            Assert.IsType<Exception>(ex);
            Assert.Equal("Some exception occurred", ex.Message);
        }

        #endregion
    }
}
