using Newtonsoft.Json;
using System;

namespace PG.ABBs.Provider.Ciam
{
    public class CiamBase
    {
        [JsonProperty(PropertyName = "consumerID")]
        public string ConsumerId { get; set; }
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
