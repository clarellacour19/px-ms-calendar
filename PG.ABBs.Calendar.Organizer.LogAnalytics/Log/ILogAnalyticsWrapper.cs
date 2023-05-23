// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogAnalyticsWrapper.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the ILogAnalyticsWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogAnalytics.Log
{
    using System.Collections.Generic;

    public interface ILogAnalyticsWrapper
    {
        #region Public Methods and Operators

        void SendLogEntries<T>(List<T> entities, string logType);

        void SendLogEntry<T>(T entity, string logType);

        string GetLocal();

        #endregion
    }
}
