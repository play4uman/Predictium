using Predictium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictium.Output
{
    public interface ICanOutput
    {
        public Task Output(IList<PredictionModel> predictionModels); 
    }
}
