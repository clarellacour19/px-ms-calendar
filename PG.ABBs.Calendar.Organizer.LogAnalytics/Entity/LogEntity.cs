// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntity.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the LogEntity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogAnalytics.Entity
{
    using Newtonsoft.Json;

    [JsonObject]
    public class LogEntity
    {
        #region Public Properties

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("Local")]
        public string Local { get; set; }

        #endregion
    }
}