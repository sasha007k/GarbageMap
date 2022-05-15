using Newtonsoft.Json;

namespace GarbageMap.Models.ApiModels
{
    public class Location
    {
        [JsonProperty("lat")]
        public float Latitude { get; set; }

        [JsonProperty("lng")]
        public float Longitude { get; set; }
    }
}
