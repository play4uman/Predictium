using Predictium.Cofiguration;
using Predictium.Predictors.Scraped;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Predictium
{
    public class ConfigurationConstants
    {
        public static readonly string CryptoGroundName = "Cryptoground.com";
        public static readonly string CryptoGroundScrapeUrl = "https://www.cryptoground.com/ethereum-price-prediction";
    }
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var cg = new CryptoGroundDotCom(new ScrapePredictorConfiguration { ScrapeEthUrl = ConfigurationConstants.CryptoGroundScrapeUrl });
            var result = await cg.GetTommorowPrediction(Models.CurrencyType.ETH);
        }
    }
}
