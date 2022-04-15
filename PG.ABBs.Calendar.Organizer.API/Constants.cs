// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Constants for JobApi project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace PG.ABBs.Calendar.Organizer.API
{
    using System;

    public static class Constants
    {
        #region Constants

        public const string Separator = ";..;";

        #endregion

        #region Public Methods and Operators

        public static string GetFormCacheKey(string formId, string userToken, string local)
        {
            return $"GetForm_{formId}_{userToken}_{local}";
        }

        #endregion

        public struct ErrorCodes
        {
            #region Constants

            public const string BadParameters = "BadParameters";

            public const string Failed = "Failed";

            public const string Ko = "KO";

            public const string NoResultsFound = "NoResultsFound";

            public const string Ok = "OK";

            public const string UnexpectedError = "UnexpectedError";

            #endregion
        }

        public struct JanrainService
        {
            #region  Access token And Refresh token

            public const string Code = "code";

            public const string RefreshToken = "refresh_token";

            #endregion
        }

        public struct ProcedureName
        {
            #region Constants

            public const string GetForm = "GETFORM";

            public const string GetErrorByLocaleAndKeyAndDefault = "GetErrorByLocaleAndKeyAndDefault";

            public const string GetSocialRegistrationForm = "GetSocialRegistrationForm";

            #endregion
        }

        public struct ProcedureParameterName
        {

            public const string FormId = "FormId";

            public const string FormType = "FormType";

            public const string Local = "Formlocal";

            public const string ErrorLocale = "Local";

            public const string ErrorKey = "ErrorKey";

            public const string ErrorDefaultMessage = "DefaultValue";

            public const string Culture = "culture";
        }

        private static readonly Guid SocialRegistrationTypeId = new Guid("B3B7B580-9FB5-45DF-9913-19DE0FC45713");

        public static string GetFormCacheKey(string formId, string local)
        {
            return $"GetForm_{formId}_{local}";
        }

        public static Guid SocialRegistrationFormId()
        {
            return SocialRegistrationTypeId;
        }

        public const string Authority = "JwtSettings:Authority";
        public const string Audience = "JwtSettings:Audience";
        public const string ClientSecret = "JwtSettings:ClientSecret";
        public const string ClientID = "JwtSettings:ClientID";
    }
}
