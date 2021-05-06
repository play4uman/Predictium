using Predictium.Predictors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Predictium.Models
{
    public class PredictionModel
    {
        public CurrencyType Currency { get; set; }
        public IPredictor Author { get; set; }
        public DateTime Date { get; set; }
        public double AveragePrice { get; set; }
        public double ChangePercent{ get; set; }
    }
}
