using Predictium.Configuration;
using Predictium.Models;
using Predictium.Output.Sheets;
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
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var cg = new CryptoGroundDotCom(new ScrapePredictorConfiguration
            {
                ScrapeUrls =
            {
                { CryptoCurrencyType.ETH, ConfigurationConstants.CryptoGroundEthScrapeUrl },
                { CryptoCurrencyType.DOT, ConfigurationConstants.CryptoGroundDotScrapeUrl },
                { CryptoCurrencyType.BTC, ConfigurationConstants.CryptoGroundBtcScrapeUrl }
            }
            });
            var tr = new ThirtyRatesDotCom(new ScrapePredictorConfiguration
            {
                ScrapeUrls = new Dictionary<Models.CryptoCurrencyType, string>
            {
                { CryptoCurrencyType.ETH, ConfigurationConstants.ThirtyRatesEthScrapeUrl },
                { CryptoCurrencyType.DOT, ConfigurationConstants.ThirtyRatesDotScrapeUrl },
                { CryptoCurrencyType.BTC, ConfigurationConstants.ThirtyRatesBtcScrapeUrl }
            }
            });

            var wi = new WalletInvestorDotCom(new ScrapePredictorConfiguration
            {
                ScrapeUrls = new Dictionary<Models.CryptoCurrencyType, string>
            {
                { CryptoCurrencyType.ETH, ConfigurationConstants.WalletInvestorEthScrapeUrl },
                { CryptoCurrencyType.DOT, ConfigurationConstants.WalletInvestorDotScrapeUrl },
                { CryptoCurrencyType.BTC, ConfigurationConstants.WalletInvestorBtcScrapeUrl }
            }
            });

            var reality = new BinanceRealityMonitor();

            var resultCgEth = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.ETH);
            var resultCgBtc = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.BTC);
            var resultCgDot = await cg.GetTommorowPredictionAsync(CryptoCurrencyType.DOT);
            var resultTrEth = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.ETH);
            var resultTrBtc = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.BTC);
            var resultTrDot = await tr.GetTommorowPredictionAsync(CryptoCurrencyType.DOT);
            var resultWiEth = await wi.GetTommorowPredictionAsync(CryptoCurrencyType.ETH);
            var resultWiBtc = await wi.GetTommorowPredictionAsync(CryptoCurrencyType.BTC);
            var resultWiDot = await wi.GetTommorowPredictionAsync(CryptoCurrencyType.DOT);

            var resultReality = await reality.GetPriceNowAsync(CryptoCurrencyType.ETH);

            var results = new[] { resultCgEth, resultCgBtc, resultCgDot, resultTrBtc, resultTrEth,
                resultTrDot, resultWiEth, resultWiBtc, resultWiDot };

            var output = new SheetsOutput();
            await output.InitializeAsync();
            await output.Output(results);
        }
    }
}
