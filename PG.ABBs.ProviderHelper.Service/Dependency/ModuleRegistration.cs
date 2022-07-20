// <copyright file="ModuleRegistration.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the ModuleRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Webservices.Service.Bng.Dependency
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PG.ABBs.Calendar.Organizer.DependencyResolution.Registries;
    using PG.ABBs.ProviderHelper.Service;
    using System.Collections.Generic;

    public class ModuleRegistration : IDependency
    {
        #region Public Methods and Operators

        public void Register(IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
        {
            services.Configure<List<JanrainProvider>>(configuration.GetSection(ProviderConstant.JanrainProviders));

           

        }

        #endregion
    }
}