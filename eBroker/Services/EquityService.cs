using eBroker.Contracts;
using eBroker.Models;
using Repository;
using Repository.Entities;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Services
{
    public class EquityService : IEquityService
    {
        readonly decimal minBrokerage = 20;

        readonly ISystemClock _systemClock;
        readonly IGenericRepository<EquityTransaction> _equityTransactionRepository;
        readonly IGenericRepository<EquityHolding> _equityHoldingRepository;
        readonly IFundService _fundServices;
        public EquityService(IGenericRepository<EquityTransaction> equityTransactionRepository, IGenericRepository<EquityHolding> equityHoldingRepository,  IFundService fundServices, ISystemClock systemClock)
        {
            _equityTransactionRepository = equityTransactionRepository;
            _fundServices = fundServices;
            _equityHoldingRepository = equityHoldingRepository;
            _systemClock = systemClock;
        }

        public async Task<int> BuyEquityAsync(EquityTransactionRequest equityTransactionRequest)
        {
            DateTime now = _systemClock.GetCurrentTime();
            bool isHoldingAdded = false;
            EquityHolding currentHolding;

            try
            {
                if (!Utility.IsValidTransactionTime(now))
                {
                    throw new InvalidOperationException("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.");
                }

                Account account = await _fundServices.GetAccountAsync(equityTransactionRequest.AccountId);

                var requiredFunds = equityTransactionRequest.PricePerUnit * equityTransactionRequest.UnitsToBuySell;

                if (requiredFunds <= account.Balance)
                {
                    var currentHoldings = await _equityHoldingRepository.GetAsync(x => x.CustomerId == equityTransactionRequest.UserId && x.EquityId == equityTransactionRequest.EquityId);

                    if (currentHoldings.Count() > 0)
                    {
                        currentHolding = currentHoldings.First();
                        currentHolding.Units += equityTransactionRequest.UnitsToBuySell;

                        _equityHoldingRepository.Update(currentHolding);
                    }
                    else
                    {
                        currentHolding = new EquityHolding
                        {
                            EquityId = equityTransactionRequest.EquityId,
                            CustomerId = equityTransactionRequest.UserId,
                            Units = equityTransactionRequest.UnitsToBuySell
                        };

                        await _equityHoldingRepository.InsertAsync(currentHolding);

                        isHoldingAdded = true;
                    }

                    _equityHoldingRepository.Save();

                    try
                    {
                        AddFundRequest addFundRequest = new AddFundRequest
                        {
                            Amount = -(requiredFunds),
                            CustomerId = equityTransactionRequest.UserId
                        };

                        var result = await _fundServices.AddFundsAsync(addFundRequest);
                    }
                    catch (Exception ex)
                    {
                        if (isHoldingAdded)
                            _equityHoldingRepository.Delete(currentHolding);
                        else
                        {
                            currentHolding.Units -= equityTransactionRequest.UnitsToBuySell;
                            _equityHoldingRepository.Update(currentHolding);
                        }
                        _equityHoldingRepository.Save();
                        throw;
                    }

                    EquityTransaction equityTransaction = new EquityTransaction
                    {
                        CustomerId = equityTransactionRequest.UserId,
                        EquityId = equityTransactionRequest.EquityId,
                        Units = equityTransactionRequest.UnitsToBuySell,
                        PricePerUnit = equityTransactionRequest.PricePerUnit,
                        TransactionType = TransactionType.Buy
                    };

                    equityTransaction = await _equityTransactionRepository.InsertAsync(equityTransaction);

                    _equityTransactionRepository.Save();

                    return equityTransaction.Id;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds. Please add funds to your account to buy Equity.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<int> SellEquityAsync(EquityTransactionRequest equityTransactionRequest)
        {
            DateTime now = _systemClock.GetCurrentTime();
            EquityHolding currentHolding;

            try
            {
                if (!Utility.IsValidTransactionTime(now))
                {
                    throw new InvalidOperationException("Buying/Selling Equity is only allowed from Monday to Friday between 9am to 3pm.");
                }

                var currentHoldings = await _equityHoldingRepository.GetAsync(x => x.CustomerId == equityTransactionRequest.UserId && x.EquityId == equityTransactionRequest.EquityId);

                if (currentHoldings.Count() > 0)
                {
                    currentHolding = currentHoldings.First();

                    if (currentHolding.Units >= equityTransactionRequest.UnitsToBuySell)
                    {
                        currentHolding.Units -= equityTransactionRequest.UnitsToBuySell;

                        _equityHoldingRepository.Update(currentHolding);
                        _equityHoldingRepository.Save();

                        var totalSellAmount = equityTransactionRequest.PricePerUnit * equityTransactionRequest.UnitsToBuySell;

                        var brokerageByPercent = totalSellAmount * Convert.ToDecimal(0.0005);
                        var brokerage = brokerageByPercent > minBrokerage ? brokerageByPercent : minBrokerage;

                        try
                        {
                            AddFundRequest addFundRequest = new AddFundRequest
                            {
                                Amount = totalSellAmount - brokerage,
                                CustomerId = equityTransactionRequest.UserId
                            };

                            var result = await _fundServices.AddFundsAsync(addFundRequest);
                        }
                        catch (Exception ex)
                        {
                            currentHolding.Units += equityTransactionRequest.UnitsToBuySell;
                            _equityHoldingRepository.Update(currentHolding);
                            
                            _equityHoldingRepository.Save();
                            
                            throw;
                        }

                        EquityTransaction equityTransaction = new EquityTransaction
                        {
                            CustomerId = equityTransactionRequest.UserId,
                            EquityId = equityTransactionRequest.EquityId,
                            Units = equityTransactionRequest.UnitsToBuySell,
                            PricePerUnit = equityTransactionRequest.PricePerUnit,
                            TransactionType = TransactionType.Sell
                        };

                        equityTransaction = await _equityTransactionRepository.InsertAsync(equityTransaction);
                        
                        _equityTransactionRepository.Save();
                        
                        return equityTransaction.Id;
                    }
                    else
                    {
                        throw new InvalidOperationException("Equity Units to sell are greater than Current Equity unit holding.");
                    }

                }
                else
                {
                    throw new InvalidOperationException("No Equity units in holding to sell.");
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
