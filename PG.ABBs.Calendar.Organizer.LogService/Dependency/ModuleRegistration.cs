// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleRegistration.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the ModuleRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogService.Dependency
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using PG.ABBs.Calendar.Organizer.DependencyResolution.Registries;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Constant;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Log;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Settings;
    using PG.ABBs.Calendar.Organizer.LogService.Extension;

    public class ModuleRegistration : IDependency
    {
        #region Public Methods and Operators

        public void Register(IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            #region Settings

            services.Configure<LogAnalyticsSettings>(configuration.GetSection(Constant.LogSettings));

            services.AddScoped(config => config.GetService<IOptionsSnapshot<LogAnalyticsSettings>>().Value);

            #endregion

            #region Services

            services.AddScoped<ILogAnalyticsWrapper, LogAnalyticsWrapper>();

            #endregion

            #region Logging

            services.AddLogging(
                loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                    loggingBuilder.AddAzureLogger();
                });

            #endregion
        }

        #endregion
    }
}
