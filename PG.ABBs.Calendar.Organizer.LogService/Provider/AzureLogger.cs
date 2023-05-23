// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogger.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the AzureLogProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogService.Provider
{
    using System;
    using Microsoft.Extensions.Logging;

    using PG.ABBs.Calendar.Organizer.LogAnalytics.Configuration;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Entity;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Log;

    public class AzureLogger : ILogger
    {
        #region Fields

        private readonly AzureLogConfiguration config;

        private readonly ILogAnalyticsWrapper logWrapper;

        private readonly string name;

        #endregion

        #region Constructors and Destructors

        public AzureLogger(string name, AzureLogConfiguration config, ILogAnalyticsWrapper logAnalytics)
        {
            this.name = name;
            this.config = config;
            this.logWrapper = logAnalytics; 
        }

        #endregion

        #region Public Methods and Operators

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= this.config.LogLevel;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (this.config.EventId == 0 || this.config.EventId == eventId.Id)
            {
                try
                {
                    var logEntity = new LogEntity
                    {
                        Level = logLevel.ToString(),
                        Message =
                            $"{logLevel} - {this.logWrapper.GetLocal()} - {eventId.Id} - {this.name} - {formatter(state, exception)}",
                        Local = this.logWrapper.GetLocal()
                    };

                    this.logWrapper.SendLogEntry(logEntity, "AzureLog");
                }
                catch (Exception ex)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{logLevel} - {eventId.Id} - {this.name} - {formatter(state, exception)} {ex}");
                    Console.ForegroundColor = color;
                }
            }
        }

        #endregion
    }
}
