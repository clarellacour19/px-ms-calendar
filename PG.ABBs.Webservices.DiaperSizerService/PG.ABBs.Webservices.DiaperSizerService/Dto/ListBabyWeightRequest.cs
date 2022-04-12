using Newtonsoft.Json;

namespace PG.ABBs.Webservices.DiaperSizerService.Dto
{
    [JsonObject]
    public class ListBabyWeightRequest
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}
