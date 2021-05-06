using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Predictors
{
    public class ScrapeException : Exception
    {
        public ScrapeException(string predictorName)
        {
            PredictorName = predictorName;
        }

        public string PredictorName { get; set; }
    }
}
