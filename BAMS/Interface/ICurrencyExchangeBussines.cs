using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAMS.Interface
{
    public interface ICurrencyExchangeBussines
    {
        Task<string> GetCurrencyExchangeAsync(string curr);
    }
}
