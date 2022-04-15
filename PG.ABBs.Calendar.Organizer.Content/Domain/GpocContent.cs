// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpocContent.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Gpoc content mapping to the content provider item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
    using Contentful.Core.Models;

    public class GpocContent
    {
        public SystemProperties Sys { get; set; }

        public string Path { get; set; }

        public string GpocId { get; set; }
    }
}