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
    public class FundServiceTest
    {

        public FundServiceTest()
        { }

        [Fact]
        public async Task AddFund_SuccessfullAsync()
        {
            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();
            mockAccountRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<Account, bool>> filter, Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy, string includeProperties) =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts.Add(new Account
                    {
                        Id = 1,
                        CustomerId = 1,
                        CustomerName = "Shashank",
                        Balance = 0,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return accounts;
                });

            mockAccountRepository.Setup(x => x.Update(It.IsAny<Account>()));
            mockAccountRepository.Setup(x => x.Save());

            IFundService fundService = new FundService(mockAccountRepository.Object);

            AddFundRequest addFundRequest = new AddFundRequest
            {
                Amount = 1000,
                CustomerId = 1
            };

            var result = await fundService.AddFundsAsync(addFundRequest);
            Assert.True(result);
        }

        [Fact]
        public async Task AddFund_GreaterThan100KSuccessfullAsync()
        {
            AddFundRequest addFundRequest = new AddFundRequest
            {
                Amount = 100001,
                CustomerId = 1
            };

            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();
            mockAccountRepository
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<Account, bool>> filter, Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy, string includeProperties) =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts.Add(new Account
                    {
                        Id = 1,
                        CustomerId = 1,
                        CustomerName = "Shashank",
                        Balance = 0,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                    });

                    return accounts;
                });

            mockAccountRepository.Setup(x => x.Update(It.IsAny<Account>()));
            mockAccountRepository.Setup(x => x.Save());

            IFundService fundService = new FundService(mockAccountRepository.Object);

            var result = await fundService.AddFundsAsync(addFundRequest);
            Assert.True(result);
        }

        [Fact]
        public async Task AddFund_NoAccountAsync()
        {
            AddFundRequest addFundRequest = new AddFundRequest
            {
                Amount = 1000,
                CustomerId = 5
            };

            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();
            mockAccountRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException($"User account not found for customer Id: {addFundRequest.CustomerId}"));

            IFundService fundService = new FundService(mockAccountRepository.Object);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => fundService.AddFundsAsync(addFundRequest));
            Assert.Equal($"User account not found for customer Id: {addFundRequest.CustomerId}", ex.Message);
        }

        [Fact]
        public async Task AddFund_GetAccountAsync()
        {
            int customerId = 1;
            Account account = new Account
            {
                Id = 1,
                CustomerId = customerId,
                CustomerName = "Shashank",
                Balance = 0,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };

            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();

            mockAccountRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<Account, bool>> filter, Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy, string includeProperties) =>
                {
                    List<Account> accounts = new List<Account>();
                    accounts.Add(account);

                    return accounts;
                });

            IFundService fundService = new FundService(mockAccountRepository.Object);

            Account accountFetched = await fundService.GetAccountAsync(customerId);

            Assert.Equal(account, accountFetched);
            Assert.Equal(account.Id, accountFetched.Id);
            Assert.Equal(account.Balance, accountFetched.Balance);
        }

        [Fact]
        public async Task AddFund_GetAccount_AccountNotFoundAsync()
        {
            int customerId = 10;

            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();

            mockAccountRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ReturnsAsync((Expression<Func<Account, bool>> filter, Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy, string includeProperties) =>
                {
                    return new List<Account>();
                });

            IFundService fundService = new FundService(mockAccountRepository.Object);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => fundService.GetAccountAsync(customerId));
            Assert.Equal($"User account not found for customer Id: {customerId}", ex.Message);

        }

        [Fact]
        public async Task AddFund_GetAccount_FailedAsync()
        {
            int customerId = 10;

            Mock<IGenericRepository<Account>> mockAccountRepository = new Mock<IGenericRepository<Account>>();

            mockAccountRepository.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<Func<IQueryable<Account>, IOrderedQueryable<Account>>>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException($"User account not found for customer Id: {customerId}"));

            IFundService fundService = new FundService(mockAccountRepository.Object);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => fundService.GetAccountAsync(customerId));
            Assert.Equal($"User account not found for customer Id: {customerId}", ex.Message);
        }

        //private void Seed(EBrokerDBContext context)
        //{
        //    context.Accounts.Add(new Account
        //    {
        //        Id = 1,
        //        CustomerId = 1,
        //        CustomerName = "Shashank",
        //        Balance = 0,
        //        IsActive = true,
        //        CreatedOn = DateTime.Now,
        //        ModifiedOn = DateTime.Now,
        //    });

        //    context.Accounts.Add(new Account
        //    {
        //        Id = 2,
        //        CustomerId = 2,
        //        CustomerName = "Test User 1",
        //        Balance = 100,
        //        IsActive = true,
        //        CreatedOn = DateTime.Now,
        //        ModifiedOn = DateTime.Now,
        //    });

        //    context.Accounts.Add(new Account
        //    {
        //        Id = 3,
        //        CustomerId = 3,
        //        CustomerName = "Test User 2",
        //        Balance = 3000,
        //        IsActive = true,
        //        CreatedOn = DateTime.Now,
        //        ModifiedOn = DateTime.Now,
        //    });

        //    context.EquityHoldings.Add(new EquityHolding
        //    {
        //        Id = 1,
        //        CustomerId = 1,
        //        EquityId = 1,
        //        CreatedOn = DateTime.Now,
        //        ModifiedOn = DateTime.Now,
        //    });

        //    context.SaveChanges();

        //}
    }
}
