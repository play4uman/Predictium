using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Predictium.Configuration
{
    public class ScrapePredictorConfiguration
    {
        public Dictionary<CryptoCurrencyType, string> ScrapeUrls { get; set; } = new Dictionary<CryptoCurrencyType, string>();
    }
}
