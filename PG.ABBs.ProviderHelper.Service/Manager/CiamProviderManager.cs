// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CiamProviderManager.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the CiamProviderManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.ProviderHelper
{
    using Microsoft.Extensions.Options;
    using PG.ABBs.Provider.Ciam.CiamProvider;
    using PG.ABBs.Provider.Ciam.Entity;
    using PG.ABBs.ProviderHelper.Service;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CiamProviderManager
    {
        #region Fields

        private readonly CiamProviders ciamProviderSettings;
        private readonly IEnumerable<ICiamProvider> providers;
        private readonly List<CiamMarket> _markets;

        #endregion

        #region Constructors and Destructors

        public CiamProviderManager(
            IOptions<CiamProviders> providers, 
            IEnumerable<ICiamProvider> providerInstances,
            IOptions<List<CiamMarket>> markets)
        {
            this.ciamProviderSettings = providers.Value;
            this.providers = providerInstances;
            this._markets = markets.Value;
        }

        #endregion


        #region Public Methods and Operators

        public ICiamProvider GetMarketProvider(string locale)
        {
            if (_markets == null
                || !_markets.Any(x => x.Locale.Equals(locale, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception($"No such market configured for locale {locale}");

            var market = _markets.Single(
                x => x.Locale.Equals(locale, StringComparison.InvariantCultureIgnoreCase));

            return this.GetProvider(market.ProviderName);
        }
        public Provider GetProviderSettings(string locale)
        {
            var market = this.GetMarketSettings(locale);
            var providerKey = market.ProviderName;

            if (this.ciamProviderSettings?.Providers == null || !this.ciamProviderSettings.Providers.Any()
                || !this.ciamProviderSettings.Providers.Any(
                    x => x.Name.Contains(providerKey, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception($"No such provider defined for {providerKey}");

            var provider = this.ciamProviderSettings.Providers.Single(
                x => x.Name.Equals(providerKey, StringComparison.InvariantCultureIgnoreCase));
            return provider;
        }
            #endregion

            #region Methods

            private ICiamProvider GetProvider(string providerKey)
        {
            if (this.ciamProviderSettings?.Providers == null || !this.ciamProviderSettings.Providers.Any()
                || !this.ciamProviderSettings.Providers.Any(
                    x => providerKey.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception($"No such provider defined for {providerKey}");

            if (this.providers == null || !this.providers.Any() || !this.providers.Any(
                    x => providerKey.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception($"No such provider assembly defined for {providerKey}");

            var provider = this.providers.FirstOrDefault(
                x => providerKey.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase));
            if (provider == null)
                throw new Exception($"No provider found for provider key {providerKey}");

            return provider;
        }
        private CiamMarket GetMarketSettings(string locale)
        {
            // Fix for HK only
            if (!string.IsNullOrEmpty(locale) && locale.Equals("zh-HK"))
            {
                locale = "zh-Hant-HK";
            }

            if (_markets == null
                || !_markets.Any(x => x.Locale.Equals(locale, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception($"No such market configured for locale {locale}");

            return _markets.Single(x => x.Locale.Equals(locale, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}