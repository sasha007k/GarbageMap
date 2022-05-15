using Microsoft.AspNetCore.Http;

namespace GarbageMap.GarbageDetection.Helpers
{
    public class DetectModel
    {
        public IFormFile ImageFile { get; set; }
        public int MinProbabilityToShow { get; set; }
        public int MinThresholdToShow { get; set; }
    }
}
