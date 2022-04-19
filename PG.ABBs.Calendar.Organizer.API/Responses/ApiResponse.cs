// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiResponse.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the ApiResponse object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.API
{
    using System;
    using static PG.ABBs.Calendar.Organizer.API.Constants;

    public class ApiResponse
    {
        #region Public Methods and Operators

        public static ApiResponse GetBadParametersResponse()
        {
            ApiResponse result = new ApiResponse();
            result.Result = ErrorCodes.BadParameters;
            return result;
        }

        public static ApiResponse GetFailedResponse()
        {
            ApiResponse result = new ApiResponse();
            result.Result = ErrorCodes.Failed;
            return result;
        }

        public static ApiResponse GetNoResultsFoundResponse()
        {
            ApiResponse result = new ApiResponse();
            result.Result = ErrorCodes.NoResultsFound;
            return result;
        }

        public static ApiResponse GetOkResponse()
        {
            ApiResponse result = new ApiResponse();
            result.Result = ErrorCodes.Ok;
            return result;
        }

        public static ApiResponse GetUnexpectedErrorResponse()
        {
            ApiResponse result = new ApiResponse();
            result.Result = ErrorCodes.UnexpectedError;
            return result;
        }

        public void UpdateResult(string message, object data)
        {
            this.Data = data;
            this.Result = message;
        }

        public void UpdateResult(string result, string message, object data)
        {
            this.Result = result;
            this.ResultDescription = message;
            this.Data = data;
        }

        public void UpdateResultWithException(string message, Exception ex)
        {
            this.Result = message;
            this.Exception = ex.Message;
            this.ResultDescription = ex.StackTrace;
        }

        public void UpdateResultWithException(string result, Exception exception, object data)
        {
            this.Result = result;
            this.ResultDescription = exception.StackTrace;
            this.Data = data;
            this.Exception = exception.Message;
        }

        #endregion

        #region Public properties

        public string Result { get; set; }

        public string ResultDescription { get; set; }

        public string Exception { get; set; }

        public object Data { get; set; }

        #endregion

        
    }
}
