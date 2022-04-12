using System;

namespace PG.ABBs.Webservices.DiaperSizerService.Models
{
    public class DiaperFitFinder
    {
        public Guid DiaperFitFinderID { get; set; }
        public MarketInfo MarketInfo { get; set; }
        public int BabyWeightValue { get; set; }
        public string BabyWeightDescription { get; set; }
        
        public string WeightRange { get; set; }
        public string AverageDiapersPerDay { get; set; }
        public decimal LastAroundMonths { get; set; }
    }
}
