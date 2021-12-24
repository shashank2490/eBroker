using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eBroker_Test
{
    public class GenericRepositoryTest
    {
        DbContextOptions<EBrokerDBContext> options;

        public GenericRepositoryTest()
        {
            options = new DbContextOptionsBuilder<EBrokerDBContext>().UseInMemoryDatabase($"EBrokerDB{DateTime.Now.Ticks}").Options;
            using (var context = new EBrokerDBContext(options))
            {
                context.Accounts.Add(new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 0,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.Accounts.Add(new Account
                {
                    Id = 2,
                    CustomerId = 2,
                    CustomerName = "Test User 1",
                    Balance = 100,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.Accounts.Add(new Account
                {
                    Id = 3,
                    CustomerId = 3,
                    CustomerName = "Test User 2",
                    Balance = 3000,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.EquityHoldings.Add(new EquityHolding
                {
                    Id = 1,
                    CustomerId = 1,
                    EquityId = 1,
                    Units=1000,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.EquityHoldings.Add(new EquityHolding
                {
                    Id = 2,
                    CustomerId = 2,
                    EquityId = 1,
                    Units = 20,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.EquityHoldings.Add(new EquityHolding
                {
                    Id = 3,
                    CustomerId = 2,
                    EquityId = 2,
                    Units = 30,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.EquityHoldings.Add(new EquityHolding
                {
                    Id = 4,
                    CustomerId = 2,
                    EquityId = 3,
                    Units = 100,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.Equities.Add(new Equity
                {
                    Id = 1,
                    EquityName = "Equity 1",
                    EquityDescription = "Equity 1",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.Equities.Add(new Equity
                {
                    Id = 2,
                    EquityName = "Equity 2",
                    EquityDescription = "Equity 2",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.Equities.Add(new Equity
                {
                    Id = 3,
                    EquityName = "Equity 3",
                    EquityDescription = "Equity 3",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetByIdAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                var account = await accountRepo.GetByIdAsync(1);

                Assert.Equal("Shashank", account.CustomerName);
            }
        }

        [Fact]
        public async Task GetByQueryAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                var accountsFetched = await accountRepo.GetAsync(x => x.CustomerId == 1);

                var actualAccounts = context.Accounts.Where(x => x.CustomerId == 1);
                Assert.True(accountsFetched.Count() == 1);
                Assert.Equal(actualAccounts.Count(), accountsFetched.Count());
                Assert.Equal(actualAccounts.First().Id, accountsFetched.First().Id);
            }
        }

        [Fact]
        public async Task GetByQuery_WithOrderByAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                var accountsFetched = await accountRepo.GetAsync(x => x.CustomerName.StartsWith("Test"), y => y.OrderBy(z => z.Balance));

                var actualAccounts = context.Accounts.Where(x => x.CustomerName.StartsWith("Test")).OrderBy(x => x.Balance);

                Assert.Equal(actualAccounts.First().Id, accountsFetched.First().Id);

                Assert.Equal("Test User 1", accountsFetched.First().CustomerName);
            }
        }

        [Fact]
        public async Task GetByQuery_WithOrderByAndIncludeAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<EquityHolding> accountRepo = new GenericRepository<EquityHolding>(context);

                var holdingsFetched = await accountRepo.GetAsync(x => x.EquityId == 1, y => y.OrderBy(z => z.CustomerId), "Equity");

                Assert.NotNull(holdingsFetched.First().Equity);

                Assert.Equal(1, holdingsFetched.First().CustomerId);
            }
        }

        [Fact]
        public async Task GetByQuery_WithIncludeAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<EquityHolding> accountRepo = new GenericRepository<EquityHolding>(context);

                var holdingsFetched = await accountRepo.GetAsync(x => x.CustomerId == 2, null, "Equity");

                Assert.NotNull(holdingsFetched.First().Equity);
                Assert.Equal(1, holdingsFetched.First().EquityId);
            }
        }

        [Fact]
        public async Task GetByOrderOnlyAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<EquityHolding> accountRepo = new GenericRepository<EquityHolding>(context);

                var holdingsFetched = await accountRepo.GetAsync(null, y => y.OrderBy(z => z.Units));

                Assert.Equal(1, holdingsFetched.First().EquityId);
                Assert.Equal(2, holdingsFetched.First().CustomerId);
            }
        }

        [Fact]
        public async Task GetDataWithIncludOnlyAsync()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<EquityHolding> accountRepo = new GenericRepository<EquityHolding>(context);

                var holdingsFetched = await accountRepo.GetAsync(null, null, "Equity");

                Assert.NotNull(holdingsFetched.First().Equity);
                Assert.Equal(1, holdingsFetched.First().EquityId);
                Assert.Equal(1, holdingsFetched.First().CustomerId);
            }
        }

        [Fact]
        public async Task InsertAsync()
        {
            Account account = new Account
            {
                Id = 4,
                CustomerId = 4,
                CustomerName = "Test User 3",
                Balance = 3000,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };

            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                await accountRepo.InsertAsync(account);
                context.SaveChanges();

                var accountFetched = context.Accounts.First(x => x.CustomerId == 4);

                Assert.Equal("Test User 3", accountFetched.CustomerName);
                Assert.Equal(4, accountFetched.Id);
            }

        }

        [Fact]
        public void Update()
        {
            Account account = new Account
            {
                Id = 3,
                CustomerId = 3,
                CustomerName = "Test User 2",
                Balance = 5000,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };

            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                accountRepo.Update(account);

                var accountFetched = context.Accounts.First(x => x.CustomerId == 3);

                Assert.Equal(5000, accountFetched.Balance);
            }
        }

        [Fact]
        public void DeleteWithObject()
        {
            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                var account = context.Accounts.First(x => x.CustomerId == 3);

                accountRepo.Delete(account);
                context.SaveChanges();

                var accountAfterDelete = context.Accounts.FirstOrDefault(x => x.CustomerId == 3);

                Assert.Null(accountAfterDelete);
            }
        }

        [Fact]
        public void DeleteWithObjectDetached()
        {
            Account account = new Account
            {
                Id = 3,
                CustomerId = 3,
                CustomerName = "Test User 2",
                Balance = 5000,
                IsActive = true,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
            };

            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);

                accountRepo.Delete(account);
                context.SaveChanges();

                var accountAfterDelete = context.Accounts.FirstOrDefault(x => x.CustomerId == 3);

                Assert.Null(accountAfterDelete);
            }
        }

        [Fact]
        public async Task DeleteWithIdAsync()
        {
            int customerId = 3;

            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);


                await accountRepo.DeleteAsync(customerId);
                context.SaveChanges();

                var accountAfterDelete = context.Accounts.FirstOrDefault(x => x.CustomerId == 3);

                Assert.Null(accountAfterDelete);
            }
        }

        [Fact]
        public async Task SaveChanges()
        {
            int customerId = 3;

            using (var context = new EBrokerDBContext(options))
            {
                GenericRepository<Account> accountRepo = new GenericRepository<Account>(context);


                await accountRepo.DeleteAsync(customerId);
                accountRepo.Save();

                var accountAfterDelete = context.Accounts.FirstOrDefault(x => x.CustomerId == 3);

                Assert.Null(accountAfterDelete);
            }
        }

    }
}
