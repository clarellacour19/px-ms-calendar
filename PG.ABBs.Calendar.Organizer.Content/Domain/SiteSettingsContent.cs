// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteSettingsContent.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Site settings mapping to the content provider item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
    using Contentful.Core.Models;

    public class SiteSettingsContent
    {
        public SystemProperties Sys { get; set; }

        public string Domain { get; set; }
    }
}
