using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PG.ABBs.Webservices.DiaperSizerService
{
    public static class Constants
    {
        public const string ConnectionString = "DatabaseSettings:ConnectionString";

        public const string DatabaseSettings = "DatabaseSettings";

        public const string Authority = "JwtSettings:Authority";
        public const string Audience = "JwtSettings:Audience";
        public const string ClientSecret = "JwtSettings:ClientSecret";
        public const string ClientID = "JwtSettings:ClientID";

        public struct ErrorCodes
        {
            public const string Ok = "OK";
            public const string Failed = "FAILED";
            public const string UnexpectedError = "UNEXPECTED_ERROR";
            public const string NoResultsFound = "NO_RESULTS_FOUND";
            public const string BadParameters = "BAD_PARAMETERS";
        }

        public struct ErrorDescription
        {
            public const string Ok = "ok";
            public const string Failed = "failed";
            public const string UnexpectedError = "unexpected error";
            public const string NoResultsFound = "no results found";

            // {0} pregnancyweek from request body
            public const string NoResultsFoundDescription = "No result found";
            public const string BadParameters = "bad parameters";
        }
    }
}
