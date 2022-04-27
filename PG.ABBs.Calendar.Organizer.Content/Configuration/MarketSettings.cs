// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarketSettings.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the market settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Configuration
{
	public class MarketSettings
	{
		public const string SectionName = "Markets";

		public string Language { get; set; }

		public string DeliveryApiKey { get; set; }

		public string PreviewApiKey { get; set; }

		public string ManagementApiKey { get; set; }

		public string SpaceId { get; set; }

		public bool UsePreviewApi { get; set; }

		public int MaxNumberOfRateLimitRetries { get; set; }

		public string Environment { get; set; }
		public string DomainName { get; set; }
		public string SiteName { get; set; }
		public int DeleteTimeSpan { get; set; }
		public string CdnPrefix { get; set; }
		public string TimeZone { get; set; }
	}
}