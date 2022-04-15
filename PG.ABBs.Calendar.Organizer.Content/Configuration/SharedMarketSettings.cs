using System;
using System.Collections.Generic;
using System.Text;

namespace PG.ABBs.Calendar.Organizer.Content.Configuration
{
	public class SharedMarketSettings
	{
		public const string SectionName = "SharedMarket";

		public string Language { get; set; }
		public string DefaultLocale { get; set; }

		public string DeliveryApiKey { get; set; }

		public string PreviewApiKey { get; set; }

		public string ManagementApiKey { get; set; }

		public string SpaceId { get; set; }

		public bool UsePreviewApi { get; set; }

		public int MaxNumberOfRateLimitRetries { get; set; }

		public string Environment { get; set; }
    }
}
