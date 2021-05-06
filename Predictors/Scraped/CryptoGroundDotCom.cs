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

        public override string ScrapePattern => @"<td class=""currency"">\s+1 day\s+<\/td>\s+<td class=\""(price up|price down)\"">\s+\$(.*?)<\/td>\s+<td class=\""(price up|price down)\"">\s+(.*?)\s+<\/td>";

        public async override Task<PredictionModel> GetTommorowPrediction(CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.ETH => await GetTommorowEthPrediction(),
                CurrencyType.BTC => throw new NotImplementedException(),
                _ => throw new Exception("Unknown currency type")
            };
        }

        private async Task<PredictionModel> GetTommorowEthPrediction()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Configuration.ScrapeEthUrl);
            var html = await response.Content.ReadAsStringAsync();
            var regex = new Regex(ScrapePattern, RegexOptions.Singleline | RegexOptions.Compiled);
            var match = regex.Match(html);

            if (!match.Success)
                throw new ScrapeException(Name);

            double tommorowPricePrediction = double.Parse(match.Groups[2].Value.Trim().Replace(",", ""), CultureInfo.InvariantCulture);
            double tommorowPercentPrediction = double.Parse(match.Groups[4].Value.Trim().Replace("%", ""), CultureInfo.InvariantCulture);

            var result = new PredictionModel
            {
                Currency = CurrencyType.ETH,
                Author = this,
                AveragePrice = tommorowPricePrediction,
                ChangePercent = tommorowPercentPrediction,
                Date = DateTime.Now.AddDays(1)
            };

            return result;
        }
    }
}
