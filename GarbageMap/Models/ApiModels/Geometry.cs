using Newtonsoft.Json;

namespace GarbageMap.Models.ApiModels
{
    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}
