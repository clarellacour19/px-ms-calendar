// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogConfiguration.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the AzureLogConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogAnalytics.Configuration
{
    using Microsoft.Extensions.Logging;
    public class AzureLogConfiguration
    {
        #region Public Properties

        public int EventId { get; set; } = 0;

        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        #endregion
    }
}