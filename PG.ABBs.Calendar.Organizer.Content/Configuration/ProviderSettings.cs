// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderSettings.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the provider settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Configuration
{
    public class ProviderSettings
    {
        #region Public Properties

        /// <summary>
        ///     Gets the SectionName.
        /// </summary>
        public static string SectionName { get; } = "Content";

        /// <summary>
        ///     Gets or sets the ProviderName.
        /// </summary>
        public string ProviderName { get; set; } = "Default";

        #endregion
    }
}