using Predictium.Configuration;
using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Predictium.Predictors.Scraped
{
    public class WalletInvestorDotCom : BaseScrapedPredictor<ScrapePredictorConfiguration>
    {
        public WalletInvestorDotCom(ScrapePredictorConfiguration configuration) : base(configuration)
        {
        }

        public override string Name => ConfigurationConstants.WalletInvestorName;
        private static string TommorowFormattedString => DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        private static string TodayPricePattern = @"Current price today: <span class=""number""><span class=\"".*?\"">.</span>(.*?) USD";

        protected override string GetScrapePattern(CryptoCurrencyType cryptoCurrencyType)
        {
            return cryptoCurrencyType switch
            {
                CryptoCurrencyType.ETH => @$"<tr class=""w0"" data-key=""0""><td class=""w0"" data-col-seq=""0"">{TommorowFormattedString}</td><td class=""w0"" data-col-seq=""1""><span class=""mobileshow"">Price: </span><i class=""fa fa-usd"" aria-hidden=""true""></i>(.*?)</td>",
                CryptoCurrencyType.DOT => @$"<tr class=""w0"" data-key=""0""><td class=""w0"" data-col-seq=""0"">{TommorowFormattedString}</td><td class=""w0"" data-col-seq=""1""><span class=""mobileshow"">Price: </span><i class=""fa fa-usd"" aria-hidden=""true""></i>(.*?)</td>",
                CryptoCurrencyType.BTC => @$"<tr class=""w0"" data-key=""0""><td class=""w0"" data-col-seq=""0"">{TommorowFormattedString}</td><td class=""w0"" data-col-seq=""1""><span class=""mobileshow"">Price: </span><i class=""fa fa-usd"" aria-hidden=""true""></i>(.*?)</td>",
                _ => throw new Exception("Unknown Crypto Currency type")
            };
        }

        protected override async Task<PredictionModel> GetTommorowBtcPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.BTC);

        protected override async Task<PredictionModel> GetTommorowDotPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.DOT);

        protected override async Task<PredictionModel> GetTommorowEthPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.ETH);

        private async Task<PredictionModel> GetGeneralTommorowPredictionAsync(CryptoCurrencyType cryptoCurrencyType)
        {
            using var httpClient = new HttpClient();
            string url = Configuration.ScrapeURLs[cryptoCurrencyType];
            var response = await httpClient.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();
            var scrapePattern = GetScrapePattern(cryptoCurrencyType);
            var regex = new Regex(scrapePattern);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPrice = double.Parse(match.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
            double todayPrice = GetTodayPrice(html);
            double tommorowPercent = (tommorowPrice / (todayPrice / 100)) - 100;

            return new PredictionModel
            {
                Author = this,
                AveragePrice = tommorowPrice,
                ChangePercent = tommorowPercent,
                CryptoCurrencyCode = cryptoCurrencyType.ToString(),
                Date = DateTime.Now.AddDays(Configuration.EarliestPredictionAvailableAfterDays),
                FiatCurrencyCode = FiatCurrencyCode
            };
        }

        private double GetTodayPrice(string html)
        {
            var regex = new Regex(TodayPricePattern);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double todayPrice = double.Parse(match.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
            return todayPrice;
        }
    }
}
