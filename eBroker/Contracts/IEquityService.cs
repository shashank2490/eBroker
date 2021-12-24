using eBroker.Models;
using System.Threading.Tasks;

namespace eBroker.Contracts
{
    public interface IEquityService
    {
        Task<int> BuyEquityAsync(EquityTransactionRequest equityTransactionRequest);
        Task<int> SellEquityAsync(EquityTransactionRequest equityTransactionRequest);
    }
}