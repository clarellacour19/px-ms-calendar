// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSettings.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Content settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Configuration
{
    using System.Collections.Generic;

    public class ContentSettings
    {
       // public const string GpocContentTypeSectionName = "GpocContentTypes";

        public const string SiteSetting = "siteSettings";

        //public const string SharedSetting = "sharedSettings";

        public const string CalendarEvent = "calendarEvent";
        public IEnumerable<MarketSettings> ContentFul { get; set; } = new List<MarketSettings>();
    }
}