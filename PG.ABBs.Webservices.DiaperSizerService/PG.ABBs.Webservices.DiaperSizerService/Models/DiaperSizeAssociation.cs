
using System;

namespace PG.ABBs.Webservices.DiaperSizerService.Models
{
    public class DiaperSizeAssociation
    {
        public Guid DiaperSizeAssociationID { get; set; }

        public Guid DiaperFitFinderID { get; set; }

        public Guid DiaperSizeID { get; set; }

        public DiaperFitFinder DiaperFitFinder { get; set; }

        public DiaperSize DiaperSize { get; set; }
    }
}
