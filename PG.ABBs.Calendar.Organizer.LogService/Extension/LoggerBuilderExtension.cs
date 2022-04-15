// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogServicesCollectionExtension.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the LogServicesCollectionExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogService.Extension
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PG.ABBs.Calendar.Organizer.LogService.Provider;

    public static class LoggerBuilderExtension
    {
        #region Public Methods and Operators

        public static ILoggingBuilder AddAzureLogger(this ILoggingBuilder loggingBuilder)
        {
            var serviceProvider = loggingBuilder.Services.BuildServiceProvider();
            loggingBuilder.AddProvider(new AzureLogProvider(serviceProvider));
            return loggingBuilder;
        }

        #endregion
    }
}
