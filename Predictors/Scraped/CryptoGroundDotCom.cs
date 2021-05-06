using Predictium.Cofiguration;
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

        public override string ScrapeEthPattern => @"<td class=""currency"">\s+1 day\s+<\/td>\s+<td class=\""(price up|price down)\"">\s+\$(.*?)<\/td>\s+<td class=\""(price up|price down)\"">\s+(.*?)\s+<\/td>";


        protected override async Task<PredictionModel> GetTommorowEthPredictionAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Configuration.ScrapeEthUrl);
            var html = await response.Content.ReadAsStringAsync();
            var regex = new Regex(ScrapeEthPattern, RegexOptions.Singleline | RegexOptions.Compiled);
            var match = regex.Match(html);

            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPricePrediction = double.Parse(match.Groups[2].Value.Trim().Replace(",", ""), CultureInfo.InvariantCulture);
            double tommorowPercentPrediction = double.Parse(match.Groups[4].Value.Trim().Replace("%", ""), CultureInfo.InvariantCulture);

            var result = new PredictionModel
            {
                CryptoCurrencyCode = CryptoCurrencyType.ETH.ToString(),
                Author = this,
                AveragePrice = tommorowPricePrediction,
                ChangePercent = tommorowPercentPrediction,
                Date = DateTime.Now.AddDays(1),
                FiatCurrencyCode = this.FiatCurrencyCode
            };

            return result;
        }

        protected override async Task<PredictionModel> GetTommorowBtcPredictionAsync() =>
            throw new NotImplementedException();
    }
}
