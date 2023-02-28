using System;
using System.Collections.Generic;
using System.Text;
using static PG.ABBs.CalendarOrganizer.Core.Constants.Constants;

namespace PG.ABBs.CalendarOrganizer.Core.DTO
{
    public class ValidationResponse
    {
        public string Result { get; set; }

        public string ResultDescription { get; set; }

        public string Exception { get; set; }

        public object Data { get; set; }
        public ValidationResponse GetFailedResponse()
        {
            ValidationResponse result = new ValidationResponse();
            result.Result = ErrorCodes.BadParameters;
            return result;
        }

        public ValidationResponse GetOkResponse()
        {
            ValidationResponse result = new ValidationResponse();
            result.Result = ErrorCodes.Ok;
            return result;
        }
    }
}
