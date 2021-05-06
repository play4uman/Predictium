using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Predictors
{
    public interface IPredictorForTommorow : IPredictor
    {
        Task<PredictionModel> GetTommorowPredictionAsync(CryptoCurrencyType currencyType);
    }
}
