using Newtonsoft.Json;

namespace GarbageMap.Models.ViewModels
{
    public class TreeViewNode
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("parent")]
        public string ParentId { get; set; }

        [JsonProperty("text")]
        public string Caption { get; set; }
    }
}
