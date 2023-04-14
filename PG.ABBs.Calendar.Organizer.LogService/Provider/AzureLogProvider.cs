// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogProvider.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the AzureLogProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogService.Provider
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using PG.ABBs.Calendar.Organizer.LogAnalytics.Configuration;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Log;

    public class AzureLogProvider : ILoggerProvider
    {
        #region Fields

        private readonly AzureLogConfiguration config;

        private readonly ConcurrentDictionary<string, AzureLogger> loggers =
            new ConcurrentDictionary<string, AzureLogger>();

        private readonly ILogAnalyticsWrapper logWrapper;

        #endregion

        #region Constructors and Destructors

        public AzureLogProvider(IServiceProvider serviceProvider)
        {
            this.logWrapper = serviceProvider.GetRequiredService<ILogAnalyticsWrapper>();
            this.config = new AzureLogConfiguration { LogLevel = LogLevel.Warning };
        }

        #endregion

        #region Public Methods and Operators

        public ILogger CreateLogger(string categoryName)
        {
            return this.loggers.GetOrAdd(categoryName, name => new AzureLogger(name, this.config, this.logWrapper));
        }

        public void Dispose()
        {
            this.loggers.Clear();
        }

        #endregion
    }
}
