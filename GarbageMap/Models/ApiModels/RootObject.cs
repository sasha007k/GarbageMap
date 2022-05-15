using Newtonsoft.Json;

namespace GarbageMap.Models.ApiModels
{
    public class RootObject
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
