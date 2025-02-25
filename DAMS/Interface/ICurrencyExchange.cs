using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Interface
{
    public interface ICurrencyExchange
    {
        Task<string> GetCurrencyExchangeAsync( string curr);
    }
}
