using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using System;

namespace Repository
{
    public class EBrokerDBContext : DbContext
    {
        public DbSet<Equity> Equities { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<EquityHolding> EquityHoldings { get; set; }
        public DbSet<EquityTransaction> EquityTransactions { get; set; }

        public EBrokerDBContext(DbContextOptions<EBrokerDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Equity>().ToTable("Equity");
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<EquityHolding>().ToTable("EquityHolding");
            modelBuilder.Entity<EquityTransaction>().ToTable("EquityTransaction");

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    CustomerId = 1,
                    CustomerName = "Shashank",
                    Balance = 0,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                 new Account
                 {
                     Id = 2,
                     CustomerId = 2,
                     CustomerName = "Test User 1",
                     Balance = 100,
                     IsActive = true,
                     CreatedOn = DateTime.Now,
                     ModifiedOn = DateTime.Now,
                 },
                 new Account
                 {
                     Id = 3,
                     CustomerId = 3,
                     CustomerName = "Test User 2",
                     Balance = 3000,
                     IsActive = true,
                     CreatedOn = DateTime.Now,
                     ModifiedOn = DateTime.Now,
                 }
            );

            modelBuilder.Entity<Equity>().HasData(
                new Equity
                {
                    Id = 1,
                    EquityName = "Equity 1",
                    EquityDescription = "Equity 1",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                new Equity
                {
                    Id = 2,
                    EquityName = "Equity 2",
                    EquityDescription = "Equity 2",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                new Equity
                {
                    Id = 3,
                    EquityName = "Equity 3",
                    EquityDescription = "Equity 3",
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                }
            );

            modelBuilder.Entity<EquityHolding>().HasData(
                new EquityHolding
                {
                    Id = 1,
                    CustomerId = 1,
                    EquityId = 1,
                    Units = 1000,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                new EquityHolding
                {
                    Id = 2,
                    CustomerId = 2,
                    EquityId = 1,
                    Units = 20,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                new EquityHolding
                {
                    Id = 3,
                    CustomerId = 2,
                    EquityId = 2,
                    Units = 30,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                },
                new EquityHolding
                {
                    Id = 4,
                    CustomerId = 2,
                    EquityId = 3,
                    Units = 100,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                }
            );
        }
    }
}
