using Predictium.Configuration;
using Predictium.Models;
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
        public const string CryptoGroundName = "Cryptoground.com";
        public const string CryptoGroundEthScrapeUrl = "https://www.cryptoground.com/ethereum-price-prediction";
        public const string CryptoGroundDotScrapeUrl = "https://www.cryptoground.com/polkadot-price-prediction";
        public const string CryptoGroundBtcScrapeUrl = "https://www.cryptoground.com/bitcoin-price-prediction";
        public const string ThirtyRatesName = "30rates.com";
        public const string ThirtyRatesEthScrapeUrl = "http://30rates.com/ethereum-price-prediction-tomorrow-week-month-eth-forecast";
        public const string ThirtyRatesDotScrapeUrl = "http://30rates.com/polkadot";
        public const string ThirtyRatesBtcScrapeUrl = "http://30rates.com/btc-to-usd-forecast-today-dollar-to-bitcoin";

    }
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var cg = new CryptoGroundDotCom(new ScrapePredictorConfiguration { ScrapeUrls =  
            {
                { CryptoCurrencyType.ETH, ConfigurationConstants.CryptoGroundEthScrapeUrl },
                { CryptoCurrencyType.DOT, ConfigurationConstants.CryptoGroundDotScrapeUrl },
                { CryptoCurrencyType.BTC, ConfigurationConstants.CryptoGroundBtcScrapeUrl }
            }});
            var tr = new ThirtyRatesDotCom(new ScrapePredictorConfiguration { ScrapeUrls = new Dictionary<Models.CryptoCurrencyType, string>
            {
                { CryptoCurrencyType.ETH, ConfigurationConstants.ThirtyRatesEthScrapeUrl },
                { CryptoCurrencyType.DOT, ConfigurationConstants.ThirtyRatesDotScrapeUrl },
                { CryptoCurrencyType.BTC, ConfigurationConstants.ThirtyRatesBtcScrapeUrl }
            }});
            var reality = new BinanceRealityMonitor();

            var resultCgEth = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.ETH);
            var resultCgBtc = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.BTC);
            var resultCgDot = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.DOT);
            var resultTrEth = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.ETH);
            var resultTrBtc = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.BTC);
            var resultTrDot = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.DOT);
            var resultReality = await reality.GetPriceNowAsync(CryptoCurrencyType.ETH);

            var options = new JsonSerializerOptions { WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(resultCgEth, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultCgBtc, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultCgDot, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultTrEth, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultTrBtc, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultTrDot, options));
            Console.WriteLine("----------------------------");
            Console.WriteLine(JsonSerializer.Serialize(resultReality, options));
        }
    }
}
