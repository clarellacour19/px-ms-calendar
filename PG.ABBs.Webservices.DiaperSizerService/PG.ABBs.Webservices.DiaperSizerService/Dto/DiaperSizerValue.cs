using Newtonsoft.Json;

namespace PG.ABBs.Webservices.DiaperSizerService.Dto
{
    [JsonObject]
    public class DiaperSizerValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
