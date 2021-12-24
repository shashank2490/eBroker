using eBroker.Contracts;
using eBroker.Models;
using eBroker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundsController : ControllerBase
    {
        readonly IFundService _fundServices;
        public FundsController(IFundService fundServices)
        {
            _fundServices = fundServices;
        }

        [HttpPost("AddFunds")]
        public async Task<IActionResult> AddFundsAsync([FromBody]AddFundRequest addFundRequest)
        {
            try
            {
                var result = await _fundServices.AddFundsAsync(addFundRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
