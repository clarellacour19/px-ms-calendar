using System;
using System.Collections.Generic;
using System.Text;

namespace PG.ABBs.CalendarOrganizer.Core.Constants
{
    public static class Constants
    {
        public struct ErrorCodes
        {
            public const string BadParameters = "BadParameters";

            public const string Failed = "Failed";

            public const string NoResultsFound = "NoResultsFound";

            public const string Ok = "OK";

            public const string UnexpectedError = "UnexpectedError";

            public const string Ko = "KO";

            public const string AccessTokenInvalid = "access token invalid";
            public const string AccessTokenNull = "Access token cannot be null";
        }
    }
}
