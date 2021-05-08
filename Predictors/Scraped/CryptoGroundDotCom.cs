using Predictium.Configuration;
using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Predictium.Predictors.Scraped
{
    public class CryptoGroundDotCom : BaseScrapedPredictor<ScrapePredictorConfiguration>
    {
        public CryptoGroundDotCom(ScrapePredictorConfiguration configuration) : base(configuration)
        {
        }

        public override string Name => ConfigurationConstants.CryptoGroundName;

        protected override string GetScrapePattern(CryptoCurrencyType cryptoCurrencyType)
        {
            return cryptoCurrencyType switch
            {
                CryptoCurrencyType.ETH => @"<td class=""currency"">\s+1 day\s+<\/td>\s+<td class=\""(price up|price down)\"">\s+\$(.*?)<\/td>\s+<td class=\""(price up|price down)\"">\s+(.*?)\s+<\/td>",
                CryptoCurrencyType.DOT => @"<td class=""currency"">\s+1 day\s+<\/td>\s+<td class=\""(price up|price down)\"">\s+\$(.*?)<\/td>\s+<td class=\""(price up|price down)\"">\s+(.*?)\s+<\/td>",
                CryptoCurrencyType.BTC => @"<td class=""currency"">\s+1 day\s+<\/td>\s+<td class=\""(price up|price down)\"">\s+\$(.*?)<\/td>\s+<td class=\""(price up|price down)\"">\s+(.*?)\s+<\/td>",
                _ => throw new Exception("Unknown cyptocurrency type")
            };
        }

        protected override async Task<PredictionModel> GetTommorowEthPredictionAsync() =>
            await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.ETH);

        protected override async Task<PredictionModel> GetTommorowBtcPredictionAsync() =>
            await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.BTC);

        protected override async Task<PredictionModel> GetTommorowDotPredictionAsync() =>
            await GetGeneralTommorowPredictionAsync(CryptoCurrencyType.DOT);


        private async Task<PredictionModel> GetGeneralTommorowPredictionAsync(CryptoCurrencyType cryptoCurrencyType)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Configuration.ScrapeUrls[cryptoCurrencyType]);
            var html = await response.Content.ReadAsStringAsync();
            var regex = new Regex(GetScrapePattern(cryptoCurrencyType), RegexOptions.Singleline | RegexOptions.Compiled);
            var match = regex.Match(html);

            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPricePrediction = double.Parse(match.Groups[2].Value.Trim().Replace(",", ""), CultureInfo.InvariantCulture);
            double tommorowPercentPrediction = double.Parse(match.Groups[4].Value.Trim().Replace("%", ""), CultureInfo.InvariantCulture);

            var result = new PredictionModel
            {
                CryptoCurrencyCode = cryptoCurrencyType.ToString(),
                Author = this,
                AveragePrice = tommorowPricePrediction,
                ChangePercent = tommorowPercentPrediction,
                Date = DateTime.Now.AddDays(Configuration.EarliestPredictionAvailableAfterDays),
                FiatCurrencyCode = this.FiatCurrencyCode
            };

            return result;
        }
    }
}
