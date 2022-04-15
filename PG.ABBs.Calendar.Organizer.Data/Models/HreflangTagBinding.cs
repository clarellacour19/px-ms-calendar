// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HreflangTagBinding.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   HreflangTagBinding mapping to database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class HreflangTagBinding
    {
        [Key]
        public Guid MappingId { get; set; }

        public string ItemId { get; set; }

        public string AlternateUrlId { get; set; }

        public string Url { get; set; }

        public Guid SiteId { get; set; }

        public string SiteName { get; set; }

        public string Language { get; set; }
    }
}
