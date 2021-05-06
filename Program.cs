using Predictium.Configuration;
using Predictium.Predictors.Scraped;
using Predictium.Reality;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Predictium
{
    public class ConfigurationConstants
    {
        public static readonly string CryptoGroundName = "Cryptoground.com";
        public static readonly string CryptoGroundEthScrapeUrl = "https://www.cryptoground.com/ethereum-price-prediction";
        public static readonly string ThirtyRatesName = "30rates.com";
        public static readonly string ThirtyRatesEthScrapeUrl = "http://30rates.com/ethereum-price-prediction-tomorrow-week-month-eth-forecast";

    }
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var cg = new CryptoGroundDotCom(new ScrapePredictorConfiguration { ScrapeEthUrl = ConfigurationConstants.CryptoGroundEthScrapeUrl });
            var tr = new ThirtyRatesDotCom(new ScrapePredictorConfiguration { ScrapeEthUrl = ConfigurationConstants.ThirtyRatesEthScrapeUrl });
            var reality = new BinanceRealityMonitor();

            var resultCg = await cg.GetTommorowPredictionAsync(Models.CryptoCurrencyType.ETH);
            var resultTr = await tr.GetTommorowPredictionAsync(Models.CryptoCurrencyType.ETH);
            var resultReality = await reality.GetPriceNowAsync(Models.CryptoCurrencyType.ETH);

            var options = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(resultCg, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultTr, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultReality, options));
        }
    }
}
