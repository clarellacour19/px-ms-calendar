using System;
using System.Collections.Generic;
using System.Text;

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
            result.Result = "BadParameters";
            return result;
        }

        public ValidationResponse GetOkResponse()
        {
            ValidationResponse result = new ValidationResponse();
            result.Result = "OK";
            return result;
        }
    }
}
