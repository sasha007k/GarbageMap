using System.Collections.Generic;
using System.IO;
using GarbageMap.GarbageDetection.ML.DataModels;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace GarbageMap.GarbageDetection.ML
{
    public class GarbageDetectionModelConfigurator
    {
        private readonly MLContext mlContext;
        private readonly ITransformer mlModel;

        public GarbageDetectionModelConfigurator(IOnnxModel onnxModel)
        {
            mlContext = new MLContext();
            // Model creation and pipeline definition for images needs to run just once,
            // so calling it from the constructor:
            mlModel = SetupMlNetModel(onnxModel);
        }

        private ITransformer SetupMlNetModel(IOnnxModel onnxModel)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(new List<ImageInputData>());

            var pipeline = mlContext.Transforms.ResizeImages(
                    resizing: ImageResizingEstimator.ResizingKind.Fill, 
                    outputColumnName: onnxModel.ModelInput, 
                    imageWidth: ImageSettings.ImageWidth, 
                    imageHeight: ImageSettings.ImageHeight, 
                    inputColumnName: nameof(ImageInputData.Image))
                .Append(mlContext.Transforms.ExtractPixels(
                    outputColumnName: onnxModel.ModelInput))
                .Append(mlContext.Transforms.ApplyOnnxModel(
                    modelFile: onnxModel.ModelPath, 
                    outputColumnName: onnxModel.ModelOutput, 
                    inputColumnName: onnxModel.ModelInput));

            var mlNetModel = pipeline.Fit(dataView);

            return mlNetModel;
        }

        public void SaveMLNetModel(string garbageDetectionModelPath)
        {
            // Зберігає MLModel в заданий архів, для PredictionEnginePool 
            mlContext.Model.Save(mlModel, null, garbageDetectionModelPath);
        }
    }
}
