using eBroker.Contracts;
using eBroker.Models;
using Repository;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Services
{
    public class FundService : IFundService
    {
        readonly IGenericRepository<Account> _accountRepository;
        public FundService(IGenericRepository<Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> GetAccountAsync(int customerId)
        {
            try
            {
                IEnumerable<Account> accounts = await _accountRepository.GetAsync(x => x.CustomerId == customerId);
                Account account = accounts.FirstOrDefault();

                if (account is null)
                    throw new ArgumentException($"User account not found for customer Id: {customerId}");

                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get account for customer with Id: {customerId},\n {ex}");
                throw;
            }
        }

        public async Task<bool> AddFundsAsync(AddFundRequest addFundRequest)
        {
            try
            {
                Account account = await GetAccountAsync(addFundRequest.CustomerId);

                var fundToBeAdded = addFundRequest.Amount;

                if (fundToBeAdded > 100000)
                {
                    fundToBeAdded = fundToBeAdded * Convert.ToDecimal(0.9995);
                }

                account.Balance += addFundRequest.Amount;
                _accountRepository.Update(account);

                _accountRepository.Save();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update balance: {ex}");
                throw;
            }

        }

    }
}
