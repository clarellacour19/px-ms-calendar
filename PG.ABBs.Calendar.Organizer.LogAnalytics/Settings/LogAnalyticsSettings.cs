// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogAnalyticsSettings.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the LogAnalyticsSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogAnalytics.Settings
{
    public class LogAnalyticsSettings
    {
        #region Public Properties

        public string Local { get; set; }

        public string ServiceName { get; set; }

        public string SharedKey { get; set; }

        public string WorkspaceId { get; set; }

        #endregion
    }
}
