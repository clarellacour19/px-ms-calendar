// <copyright file="ModuleRegistration.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the ModuleRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.ProviderHelper
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PG.ABBs.Calendar.Organizer.DependencyResolution.Registries;
    using PG.ABBs.Provider.Ciam.CiamProvider;
    using PG.ABBs.Provider.Ciam.Entity;
    using PG.ABBs.Provider.Ciam.Janrain;
    using PG.ABBs.ProviderHelper.Service;
    using System.Collections.Generic;

    public class ModuleRegistration : IDependency
    {
        #region Public Methods and Operators

        public void Register(IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
        {
            services.Configure<CiamProviders>(configuration.GetSection(ProviderConstant.CiamProviders));
            services.Configure<List<CiamMarket>>(configuration.GetSection(ProviderConstant.CiamMarkets));

            services.AddScoped<ICiamProvider, JanrainProvider>();

            
            services.AddScoped<CiamProviderManager>();
            services.AddScoped<IProviderService, ProviderService>();



        }

        #endregion
    }
}