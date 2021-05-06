using Predictium.Cofiguration;
using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Predictors.Scraped
{
    public abstract class BaseScrapedPredictor<TConfiguration> : IPredictorForTommorow
        where TConfiguration : ScrapePredictorConfiguration
    {
        public BaseScrapedPredictor(TConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public abstract string Name { get; }
        public abstract string ScrapeEthPattern { get; }
        public TConfiguration Configuration { get; set; }
        public virtual string FiatCurrencyCode { get => "USD"; } 
        public virtual async Task<PredictionModel> GetTommorowPredictionAsync(CryptoCurrencyType currencyType)
        {
            return currencyType switch
            {
                CryptoCurrencyType.ETH => await GetTommorowEthPredictionAsync(),
                CryptoCurrencyType.BTC => await GetTommorowBtcPredictionAsync(),
                _ => throw new Exception("Unknown currency type")
            };
        }

        protected abstract Task<PredictionModel> GetTommorowEthPredictionAsync();
        protected abstract Task<PredictionModel> GetTommorowBtcPredictionAsync();
    }
}
