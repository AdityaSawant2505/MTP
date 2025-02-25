using BAMS.Interface;
using DAMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Implemetations
{
    public class CurrencyExchangeBussines : ICurrencyExchangeBussines
    {
        private readonly ICurrencyExchange _currencyExchange;
        public CurrencyExchangeBussines(ICurrencyExchange currencyExchange)
        {
            _currencyExchange = currencyExchange;
        }

        public async Task<string> GetCurrencyExchangeAsync(string curr)
        {
            return await _currencyExchange.GetCurrencyExchangeAsync(curr);
        }
    }
}
