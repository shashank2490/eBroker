using eBroker.Contracts;
using eBroker.Models;
using eBroker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquityController : ControllerBase
    {
        readonly IEquityService _equityServices;
        public EquityController(IEquityService equityServices)
        {
            _equityServices = equityServices;
        }

        [HttpPost("BuyEquity")]
        public async Task<IActionResult> BuyEquityAsync([FromBody]EquityTransactionRequest equityTransactionRequest)
        {
            try
            {
                var result = await _equityServices.BuyEquityAsync(equityTransactionRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("SellEquity")]
        public async Task<IActionResult> SellEquityAsync(EquityTransactionRequest equityTransactionRequest)
        {
            try
            {
                var result = await _equityServices.SellEquityAsync(equityTransactionRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
