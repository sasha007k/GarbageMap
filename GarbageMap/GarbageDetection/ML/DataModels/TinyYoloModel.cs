namespace GarbageMap.GarbageDetection.ML.DataModels
{
    public class TinyYoloModel : IOnnxModel
    {
        public string ModelPath { get; private set; }

        public string ModelInput { get; } = "data";
        public string ModelOutput { get; } = "model_outputs0";

        public string[] Labels { get; } =
        {
            "garbage"
        };

        public (float, float)[] Anchors { get; } = { (1.08f, 1.19f), (3.42f, 4.41f), (6.63f, 11.38f), (9.42f, 5.11f), (16.62f, 10.52f) };

        public TinyYoloModel(string modelPath)
        {
            ModelPath = modelPath;
        }
    }
}
