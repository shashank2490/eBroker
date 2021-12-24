using eBroker.Models;
using Repository.Entities;
using System.Threading.Tasks;

namespace eBroker.Contracts
{
    public interface IFundService
    {
        Task<bool> AddFundsAsync(AddFundRequest addFundRequest);
        Task<Account> GetAccountAsync(int customerId);
    }
}