// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactory.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Client factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Contentful.Core;
    using Contentful.Core.Configuration;
    using Microsoft.Extensions.Options;
    using PG.ABBs.Calendar.Organizer.Content.Configuration;

	public class ClientFactory
	{
		private readonly IOptions<List<MarketSettings>> marketSettings;
		private readonly IOptions<SharedMarketSettings> sharedMarketSettings;


		public Dictionary<string, IContentfulClient> ClientCache { get; set; } = new Dictionary<string, IContentfulClient>();

		public Dictionary<string, IContentfulManagementClient> ManagementClientCache { get; set; } = new Dictionary<string, IContentfulManagementClient>();

		public ClientFactory(IOptions<List<MarketSettings>> marketSettings, IOptions<SharedMarketSettings> sharedMarketSettings)
		{
			this.marketSettings = marketSettings;
			this.sharedMarketSettings = sharedMarketSettings;
		}

		public IContentfulClient GetClient(string culture)
		{
			if (this.ClientCache.ContainsKey(culture))
			{
				return this.ClientCache[culture];
			}

			if (this.marketSettings.Value == null || !this.marketSettings.Value.Any())
			{
				return null;
			}

			var marketSetting = this.marketSettings.Value.FirstOrDefault(x => x.Language.Equals(culture, StringComparison.InvariantCultureIgnoreCase));

			if (marketSetting == null)
			{
				return null;
			}

			var httpClient = new HttpClient();
			var options = this.BuildOptions(marketSetting);

			var contentfulClient = new ContentfulClient(httpClient, options);

			this.ClientCache.Add(culture, contentfulClient);

			return contentfulClient;
		}


		public IContentfulClient GetSharedClient(string culture)
		{
			if (this.ClientCache.ContainsKey(culture))
			{
				return this.ClientCache[culture];
			}

			if (this.sharedMarketSettings.Value == null)
			{
				return null;
			}

			var sharedMarket = sharedMarketSettings.Value;

			if (sharedMarket == null)
			{
				return null;
			}

			var httpClient = new HttpClient();
			var options = this.BuildOptions(sharedMarket);

			var contentfulClient = new ContentfulClient(httpClient, options);

			this.ClientCache.Add(culture, contentfulClient);

			return contentfulClient;
		}


		public IContentfulManagementClient GetManagementClient(string culture)
		{
			if (this.ManagementClientCache.ContainsKey(culture))
			{
				return this.ManagementClientCache[culture];
			}

			if (this.marketSettings.Value == null || !this.marketSettings.Value.Any())
			{
				return null;
			}

			var marketSetting = this.marketSettings.Value.FirstOrDefault(x => x.Language.Equals(culture, StringComparison.InvariantCultureIgnoreCase));

			if (marketSetting == null)
			{
				return null;
			}

			var httpClient = new HttpClient();
			var options = this.BuildOptions(marketSetting);

			var contentfulClient = new ContentfulManagementClient(httpClient, options);

			this.ManagementClientCache.Add(culture, contentfulClient);

			return contentfulClient;
		}

		private ContentfulOptions BuildOptions(MarketSettings settings)
		{
			var options = new ContentfulOptions
			{
				DeliveryApiKey = settings.DeliveryApiKey,
				Environment = settings.Environment,
				PreviewApiKey = settings.PreviewApiKey,
				MaxNumberOfRateLimitRetries = settings.MaxNumberOfRateLimitRetries,
				SpaceId = settings.SpaceId,
				UsePreviewApi = settings.UsePreviewApi,
				ManagementApiKey = settings.ManagementApiKey
			};

			return options;
		}

		private ContentfulOptions BuildOptions(SharedMarketSettings settings)
		{
			var options = new ContentfulOptions
			{
				DeliveryApiKey = settings.DeliveryApiKey,
				Environment = settings.Environment,
				PreviewApiKey = settings.PreviewApiKey,
				MaxNumberOfRateLimitRetries = settings.MaxNumberOfRateLimitRetries,
				SpaceId = settings.SpaceId,
				UsePreviewApi = settings.UsePreviewApi,
				ManagementApiKey = settings.ManagementApiKey
			};

			return options;
		}
	}
}