using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;

namespace PG.ABBs.Calendar.Organizer.Content.Configuration
{
	public class MarketSettingsHelper
	{
		private readonly IOptions<List<MarketSettings>> marketSettings;

		public MarketSettingsHelper(IOptions<List<MarketSettings>> marketSettings)
		{
			this.marketSettings = marketSettings;
		}

		public bool HasValue => this.marketSettings.Value != null && this.marketSettings.Value.Any();

		public IDictionary<string, MarketSettings> MarketSettingsByCulture
			=> marketSettings.Value.GroupBy(ms => ms.Language).ToDictionary(g => g.Key, g => g.First());

		public IDictionary<string, MarketSettings> MarketSettingsBySpaceId
			=> marketSettings.Value.GroupBy(ms => ms.SpaceId).ToDictionary(g => g.Key, g => g.First());
	}
}
