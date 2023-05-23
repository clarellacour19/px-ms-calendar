// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogHelper.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the LogHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.LogAnalytics.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class LogHelper
    {
        #region Public Methods and Operators

        public static string GetAuthSignature(
            string serializedJsonObject,
            string dateString,
            string sharedKey,
            string workspaceId)
        {
            var stringToSign =
                $"POST\n{serializedJsonObject.Length}\napplication/json\nx-ms-date:{dateString}\n/api/logs";
            string signedString;

            var encoding = new ASCIIEncoding();
            var sharedKeyBytes = Convert.FromBase64String(sharedKey);
            var stringToSignBytes = encoding.GetBytes(stringToSign);
            using (var hmacsha256Encryption = new HMACSHA256(sharedKeyBytes))
            {
                var hashBytes = hmacsha256Encryption.ComputeHash(stringToSignBytes);
                signedString = Convert.ToBase64String(hashBytes);
            }

            return $"SharedKey {workspaceId}:{signedString}";
        }

        public static bool IsAlphaOnly(string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z]+$");
        }

        public static void ValidatePropertyTypes<T>(T entity)
        {
            // as of 2018-10-30, the allowed property types for log analytics, as defined here (https://docs.microsoft.com/en-us/azure/log-analytics/log-analytics-data-collector-api#record-type-and-properties) are: string, bool, double, datetime, guid.
            // anything else will be throwing an exception here.
            foreach (var propertyInfo in entity.GetType().GetProperties())
                if (propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType != typeof(bool)
                    && propertyInfo.PropertyType != typeof(double) && propertyInfo.PropertyType != typeof(DateTime)
                    && propertyInfo.PropertyType != typeof(Guid))
                    throw new ArgumentOutOfRangeException(
                        $"Property '{propertyInfo.Name}' of entity with type '{entity.GetType()}' is not one of the valid properties. Valid properties are String, Boolean, Double, DateTime, Guid.");
        }

        #endregion
    }
}