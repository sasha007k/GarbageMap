using Microsoft.ML.Data;

namespace GarbageMap.GarbageDetection.ML.DataModels
{
    public class TinyYoloPrediction : IOnnxObjectPrediction
    {
        [ColumnName("model_outputs0")]
        public float[] PredictedLabels { get; set; }
    }
}
