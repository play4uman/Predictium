using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Predictium.Configuration
{
    public class ScrapePredictorConfiguration
    {
        public string Name { get; set; }
        public Dictionary<CryptoCurrencyType, string> ScrapeURLs { get; set; }
        public int EarliestPredictionAvailableAfterDays { get; set; }
    }
}
