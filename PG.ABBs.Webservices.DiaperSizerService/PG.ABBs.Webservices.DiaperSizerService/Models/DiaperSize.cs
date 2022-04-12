
using System;

namespace PG.ABBs.Webservices.DiaperSizerService.Models
{
    public class DiaperSize
    {
        public Guid DiaperSizeID { get; set; }
        public int Size { get; set; }
        public string AverageNumberOfDiapers { get; set; }
    }
}
