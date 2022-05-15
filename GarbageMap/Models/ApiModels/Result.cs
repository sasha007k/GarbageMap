using Newtonsoft.Json;

namespace GarbageMap.Models.ApiModels
{
    public class Result
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }
}
