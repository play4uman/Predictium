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
        public abstract string ScrapePattern { get; }
        public TConfiguration Configuration { get; set; }
        public abstract Task<PredictionModel> GetTommorowPrediction(CurrencyType currencyType);
    }
}
