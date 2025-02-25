using BAMS.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace AMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
       private readonly ICurrencyExchangeBussines _currencyExchangeBussines;

        public CurrencyController(ICurrencyExchangeBussines currencyExchange)
        {
            _currencyExchangeBussines = currencyExchange;
        }


        [HttpGet("GetCurrencyExchangeAsync")]
        public async Task<IActionResult> GetCurrencyExchangeAsync(string currency)
        {
            try
            {
                string response = await _currencyExchangeBussines.GetCurrencyExchangeAsync(currency);
                return Ok(response); // Wrap the response in Ok() to return a 200 status
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }
}
