using Newtonsoft.Json;
using PG.ABBs.Webservices.DiaperSizerService.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PG.ABBs.Webservices.DiaperSizerService.Dto
{
    [JsonObject]
    public class BabyWeightDetails
    {
        [JsonPropertyName("weight_range")]
        public string WeightRange { get; set; }

        [JsonPropertyName("diapers_per_day")]
        public string DiapersPerDay { get; set; }

        [JsonPropertyName("last_around_months")]
        public decimal LastAroundMonths { get; set; }

        [JsonPropertyName("diaper_size")]
        public List<DiaperSize> DiaperSize { get; set; }
    }
}
