// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogProvider.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the AzureLogProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace PG.ABBs.Calendar.Organizer.LogService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using Newtonsoft.Json;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Log;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Helpers;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Settings;
    using PG.ABBs.Calendar.Organizer.LogAnalytics.Constant;

    public class LogAnalyticsWrapper : ILogAnalyticsWrapper
    {
        #region Constructors and Destructors

        public LogAnalyticsWrapper(LogAnalyticsSettings logAnalyticsSettings)
        {
            this.LogAnalyticsSettings = logAnalyticsSettings;

            if (string.IsNullOrEmpty(this.LogAnalyticsSettings.WorkspaceId))
                throw new ArgumentNullException(
                    nameof(this.LogAnalyticsSettings.WorkspaceId),
                    "workspaceId cannot be null or empty");

            if (string.IsNullOrEmpty(this.LogAnalyticsSettings.SharedKey))
                throw new ArgumentNullException(
                    nameof(this.LogAnalyticsSettings.SharedKey),
                    "sharedKey cannot be null or empty");

            this.RequestBaseUrl =
                $"https://{this.LogAnalyticsSettings.WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={Constant.ApiVersion}";
        }

        #endregion

        #region Public Properties

        public string RequestBaseUrl { get; set; }

        #endregion

        #region Properties

        private LogAnalyticsSettings LogAnalyticsSettings { get; }

        #endregion

        #region Public Methods and Operators

        public string GetLocal()
        {
            return this.LogAnalyticsSettings.Local;
        }

        public void SendLogEntries<T>(List<T> entities, string logType)
        {
            #region Argument validation

            if (entities == null)
                throw new Exception("parameter 'entities' cannot be null");

            if (logType.Length > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(logType),
                    logType.Length,
                    "The size limit for this parameter is 100 characters.");

            if (!LogHelper.IsAlphaOnly(logType))
                throw new ArgumentOutOfRangeException(
                    nameof(logType),
                    logType,
                    "Log-Type can only contain alpha characters. It does not support numerics or special characters.");

            foreach (var entity in entities)
                LogHelper.ValidatePropertyTypes(entity);

            #endregion

            var dateTimeNow = DateTime.UtcNow.ToString("r");

            var entityAsJson = JsonConvert.SerializeObject(entities);
            var authSignature = LogHelper.GetAuthSignature(
                entityAsJson,
                dateTimeNow,
                this.LogAnalyticsSettings.SharedKey,
                this.LogAnalyticsSettings.WorkspaceId);

            var finalLogType = $"{this.LogAnalyticsSettings.ServiceName}_{logType}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", authSignature);
            client.DefaultRequestHeaders.Add("Log-Type", finalLogType);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-ms-date", dateTimeNow);
            client.DefaultRequestHeaders.Add("time-generated-field", string.Empty);

            // if we want to extend this in the future to support custom date fields from the entity etc.

            HttpContent httpContent = new StringContent(entityAsJson, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var response = client.PostAsync(new Uri(this.RequestBaseUrl), httpContent).Result;
                var responseContent = response.Content;
                var result = responseContent.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request Base Url: {ex.Message} - {ex.StackTrace}");
            }
        }

        public void SendLogEntry<T>(T entity, string logType)
        {
            #region Argument validation

            if (entity == null)
                throw new Exception("parameter 'entity' cannot be null");

            if (logType.Length > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(logType),
                    logType.Length,
                    "The size limit for this parameter is 100 characters.");

            if (!LogHelper.IsAlphaOnly(logType))
                throw new ArgumentOutOfRangeException(
                    nameof(logType),
                    logType,
                    "Log-Type can only contain alpha characters. It does not support numerics or special characters.");

            LogHelper.ValidatePropertyTypes(entity);

            #endregion

            var list = new List<T> { entity };
            this.SendLogEntries(list, logType);
        }

        #endregion
    }
}
