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
    public class ThirtyRatesDotCom : BaseScrapedPredictor<ScrapePredictorConfiguration>
    {
        public ThirtyRatesDotCom(ScrapePredictorConfiguration configuration) : base(configuration)
        {
        }

        public override string Name => ConfigurationConstants.ThirtyRatesName;

        private string TommorowFormattedString =>
            DateTime.Now.AddDays(Configuration.EarliestPredictionAvailableAfterDays)
            .ToString("MM/dd", CultureInfo.InvariantCulture).Replace("/", @"\/");

        private const string TodayPricePattern = @"{0} price stood at <strong>(.*?)</strong>";

        protected override string GetScrapePattern(CryptoCurrencyType cryptoCurrencyType)
        {
            return cryptoCurrencyType switch
            {
                CryptoCurrencyType.ETH => @$"<td style=\""[^>]*\"">{TommorowFormattedString}<\/td>\s+<td style=\""[^>]*\"">.*?<\/td>\s+<td style=\""[^>]*\"">.(.*?)</td>",
                CryptoCurrencyType.BTC => @$"<td style=\""[^>]*\"">{TommorowFormattedString}<\/td>\s+<td style=\""[^>]*\"">.*?<\/td>\s+<td style=\""[^>]*\"">.(.*?)</td>",
                CryptoCurrencyType.DOT => @$"<td style=\""[^>]*\"">{TommorowFormattedString}<\/td>\s+<td style=\""[^>]*\"">.*?<\/td>\s+<td style=\""[^>]*\"">.(.*?)</td>",
                _ => throw new Exception("Unknown Crypto Currency type")
            };
        }

        protected override async Task<PredictionModel> GetTommorowEthPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.ETH);

        protected override async Task<PredictionModel> GetTommorowBtcPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.BTC);

        protected override async Task<PredictionModel> GetTommorowDotPredictionAsync()
            => await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.DOT);

        private async Task<PredictionModel> GetGeneralTommorowPredictionAsync(CryptoCurrencyType cryptoCurrencyType)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Configuration.ScrapeURLs[cryptoCurrencyType]);
            var html = await response.Content.ReadAsStringAsync();
            var scrapePattern = GetScrapePattern(cryptoCurrencyType);

            var regex = new Regex(scrapePattern, RegexOptions.Singleline | RegexOptions.Compiled);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPrice = double.Parse(match.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
            double todayPrice = GetTodayPrice(html, cryptoCurrencyType);
            double tommorowPercent = (tommorowPrice / (todayPrice / 100)) - 100;

            return new PredictionModel
            {
                Author = this,
                AveragePrice = tommorowPrice,
                ChangePercent = tommorowPercent,
                CryptoCurrencyCode = cryptoCurrencyType.ToString(),
                Date = DateTime.Now.AddDays(Configuration.EarliestPredictionAvailableAfterDays),
                FiatCurrencyCode = this.FiatCurrencyCode
            };
        }

        private double GetTodayPrice(string html, CryptoCurrencyType cryptoCurrencyType)
        {
            var todayPricePatternForCrypto = string.Format(TodayPricePattern, 
                CryptoCurrencyFullName.GetCryptoCurrencyNameByType(cryptoCurrencyType));
            var regex = new Regex(todayPricePatternForCrypto);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double todayPrice = double.Parse(match.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
            return todayPrice;
        }
    }
}
