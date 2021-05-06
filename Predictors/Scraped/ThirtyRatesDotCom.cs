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

        private static string TommorowFormattedString => DateTime.Now.AddDays(1).ToString("MM/dd", CultureInfo.InvariantCulture).Replace("/", @"\/");
        public override string ScrapeEthPattern => @$"<td style=\""[^>]*\"">{TommorowFormattedString}<\/td>\s+<td style=\""[^>]*\"">.*?<\/td>\s+<td style=\""[^>]*\"">.(.*?)</td>";
        private const string TodayPriceEthPattern = @"Ethereum price stood at <strong>(.*?)</strong>";

        protected override async Task<PredictionModel> GetTommorowEthPredictionAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Configuration.ScrapeEthUrl);
            var html = await response.Content.ReadAsStringAsync();

            var regex = new Regex(ScrapeEthPattern, RegexOptions.Singleline | RegexOptions.Compiled);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPrice = double.Parse(match.Groups[1].Value.Trim());
            double todayPrice = GetTodayEthPrice(html);
            double tommorowPercent = (tommorowPrice / (todayPrice / 100)) - 100;

            return new PredictionModel
            {
                Author = this,
                AveragePrice = tommorowPrice,
                ChangePercent = tommorowPercent,
                CryptoCurrencyCode = CryptoCurrencyType.ETH.ToString(),
                Date = DateTime.Now.AddDays(1),
                FiatCurrencyCode = this.FiatCurrencyCode
            };
        }

        protected override Task<PredictionModel> GetTommorowBtcPredictionAsync()
        {
            throw new NotImplementedException();
        }

        private double GetTodayEthPrice(string html)
        {
            var regex = new Regex(TodayPriceEthPattern);
            var match = regex.Match(html);
            if (!match.Success)
                throw new ScrapeException(Name);

            double todayPrice = double.Parse(match.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
            return todayPrice;
        }
    }
}
