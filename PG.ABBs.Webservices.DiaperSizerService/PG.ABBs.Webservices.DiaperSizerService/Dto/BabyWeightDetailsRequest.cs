

using Newtonsoft.Json;

namespace PG.ABBs.Webservices.DiaperSizerService.Dto
{
    public class BabyWeightDetailsRequest
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
