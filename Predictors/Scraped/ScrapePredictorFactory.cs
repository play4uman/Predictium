using Predictium.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Predictium.Predictors.Scraped
{
    public class ScrapePredictorFactory
    {
        public static BaseScrapedPredictor<ScrapePredictorConfiguration> GetPredictor(string name, ScrapePredictorConfiguration configuration)
        {
            return name switch
            {
                "cryptoground.com" => new CryptoGroundDotCom(configuration),
                "30rates.com" => new ThirtyRatesDotCom(configuration),
                "walletinvestor.com" => new WalletInvestorDotCom(configuration),
                _ => throw new Exception("Unknown predictor")
            };
        }
    }
}
