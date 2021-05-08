using Microsoft.Extensions.Configuration;
using Predictium.Configuration;
using Predictium.Models;
using Predictium.Output.Sheets;
using Predictium.Predictors;
using Predictium.Predictors.Scraped;
using Predictium.Reality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Predictium
{
    public class ConfigurationConstants
    {
        public const string CryptoGroundName = "cryptoground.com";
        public const string CryptoGroundEthScrapeUrl = "https://www.cryptoground.com/ethereum-price-prediction";
        public const string CryptoGroundDotScrapeUrl = "https://www.cryptoground.com/polkadot-price-prediction";
        public const string CryptoGroundBtcScrapeUrl = "https://www.cryptoground.com/bitcoin-price-prediction";
        public const string ThirtyRatesName = "30rates.com";
        public const string ThirtyRatesEthScrapeUrl = "http://30rates.com/ethereum-price-prediction-tomorrow-week-month-eth-forecast";
        public const string ThirtyRatesDotScrapeUrl = "http://30rates.com/polkadot";
        public const string ThirtyRatesBtcScrapeUrl = "http://30rates.com/btc-to-usd-forecast-today-dollar-to-bitcoin";
        public const string WalletInvestorName = "walletinvestor.com";
        public const string WalletInvestorEthScrapeUrl = "https://walletinvestor.com/forecast/ethereum-prediction";
        public const string WalletInvestorDotScrapeUrl = "https://walletinvestor.com/forecast/polkadot-prediction";
        public const string WalletInvestorBtcScrapeUrl = "https://walletinvestor.com/forecast/bitcoin-prediction";
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            MainAsync(args, configuration).Wait();
        }
        static async Task MainAsync(string[] args, IConfiguration configuration)
        {
            var configs = configuration.GetSection("ScrapePredictors")
                .Get<ScrapePredictorConfiguration[]>();

            var predictors = configs.Select(c =>
                ScrapePredictorFactory.GetPredictor(c.Name, c));

            var predictionModels = new List<PredictionModel>();
            
            var realityMonitor = new BinanceRealityMonitor();
            var registeredCryptoTypes = (CryptoCurrencyType[])Enum.GetValues(typeof(CryptoCurrencyType));
            foreach (var registeredCrypto in registeredCryptoTypes)
            {
                var currentPrice = await realityMonitor.GetPriceNowAsync(registeredCrypto);
                predictionModels.Add(currentPrice);
            }

            foreach (var predictor in predictors)
            {
                foreach (var cryptoType in predictor.Configuration.ScrapeURLs.Keys)
                {
                    try
                    {
                        var predictions = await predictor.GetTommorowPredictionAsync(cryptoType);
                        predictionModels.Add(predictions);
                    }
                    catch (ScrapeException scEx)
                    {
                        Console.WriteLine($"ScrapeException at {scEx.PredictorName}");
                    }
                }
            }

            var output = new SheetsOutput();
            await output.InitializeAsync();
            await output.Output(predictionModels);
        }
    }
}
