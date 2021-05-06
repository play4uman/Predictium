using Newtonsoft.Json;
using Predictium.Models;
using Predictium.Predictors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Reality
{
    public class BinanceRealityMonitor : IRealityMonitor, IPredictor
    {
        public const string BinanceAPIEndpoint = "https://api.binance.com/api/v3/ticker/price?symbol={0}";

        public string Name => "Binance.com";

        public async Task<PredictionModel> GetPriceNowAsync(CryptoCurrencyType cryptoCurrencyType, 
            string fiatCurrencyCode = "USD")
        {
            using var httpClient = new HttpClient();
            string symbol = GetSymbol(cryptoCurrencyType, fiatCurrencyCode);
            string requestUrl = string.Format(BinanceAPIEndpoint, symbol);

            var response = await httpClient.GetAsync(requestUrl);
            var responseText = await response.Content.ReadAsStringAsync();
            dynamic responseObj = JsonConvert.DeserializeObject(responseText);

            double price = responseObj.price;

            return new PredictionModel
            {
                Author = this,
                CryptoCurrencyCode = cryptoCurrencyType.ToString(),
                AveragePrice = price,
                ChangePercent = 0,
                Date = DateTime.Now,
                FiatCurrencyCode = fiatCurrencyCode,
                IsFuture = false
            };
        }

        private string GetSymbol(CryptoCurrencyType cryptoCurrencyType, string fiatCurrencyCode)
        {
            var postfix = fiatCurrencyCode.ToUpper() switch
            {
                "USD" => "USDT",
                "EUR" => "EUR",
                _ => fiatCurrencyCode
            };

            return $"{cryptoCurrencyType}{postfix}";
        }
    }
}
