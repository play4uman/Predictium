using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Reality
{
    public interface IRealityMonitor
    {
        public Task<PredictionModel> GetPriceNowAsync(CryptoCurrencyType cryptoCurrencyType, 
            string fiatCurrencyCode = "USD");
    }
}
