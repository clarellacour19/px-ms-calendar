using System;

namespace PG.ABBs.Webservices.DiaperSizerService.Models
{
    public class MarketInfo
    {
        public Guid MarketInfoID { get; set; }
        public string Country { get; set; }
        public string Locale { get; set; }
        public string Unit { get; set; }
    }
}
