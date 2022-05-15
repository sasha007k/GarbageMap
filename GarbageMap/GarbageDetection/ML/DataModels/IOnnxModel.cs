namespace GarbageMap.GarbageDetection.ML.DataModels
{
    public interface IOnnxModel
    {
        string ModelPath { get; }

        // To check Model input and output parameter names, you can
        // use tools like Netron: https://github.com/lutzroeder/netron
        string ModelInput { get; }
        string ModelOutput { get; }

        string[] Labels { get; }
        (float, float)[] Anchors { get; }
    }
}
