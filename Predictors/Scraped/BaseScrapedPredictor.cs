using Predictium.Configuration;
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
        public TConfiguration Configuration { get; set; }
        public virtual string FiatCurrencyCode { get => "USD"; } 
        public virtual async Task<PredictionModel> GetTommorowPredictionAsync(CryptoCurrencyType currencyType)
        {
            return currencyType switch
            {
                CryptoCurrencyType.ETH => await GetTommorowEthPredictionAsync(),
                CryptoCurrencyType.BTC => await GetTommorowBtcPredictionAsync(),
                CryptoCurrencyType.DOT => await GetTommorowDotPredictionAsync(),
                _ => throw new Exception("Unknown Crypto Currency type")
            };
        }

        protected abstract string GetScrapePattern(CryptoCurrencyType cryptoCurrencyType);
        protected abstract Task<PredictionModel> GetTommorowEthPredictionAsync();
        protected abstract Task<PredictionModel> GetTommorowBtcPredictionAsync();
        protected abstract Task<PredictionModel> GetTommorowDotPredictionAsync();
    }
}
