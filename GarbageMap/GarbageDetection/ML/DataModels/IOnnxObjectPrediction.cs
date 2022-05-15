using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageMap.GarbageDetection.ML.DataModels
{
    public interface IOnnxObjectPrediction
    {
        float[] PredictedLabels { get; set; }
    }
}
