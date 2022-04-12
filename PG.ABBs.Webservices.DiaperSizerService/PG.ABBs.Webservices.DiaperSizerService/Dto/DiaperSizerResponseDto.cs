using System.Text.Json.Serialization;
using Newtonsoft.Json;
using static PG.ABBs.Webservices.DiaperSizerService.Constants;

namespace PG.ABBs.Webservices.DiaperSizerService.Dto
{
    [JsonObject]
    public class DiaperSizerResponseDto
    {
        [JsonPropertyName("Result")]
        public string Result { get; set; }

        [JsonPropertyName("ResultDescription")]
        public string ResultDescription { get; set; }

        [JsonPropertyName("Exception")]
        public string Exception { get; set; }

        [JsonPropertyName("ResultData")]
        public object ResultData { get; set; }


        public static DiaperSizerResponseDto GetOkResponse()
        {
            DiaperSizerResponseDto result = new DiaperSizerResponseDto();
            result.Result = ErrorCodes.Ok;
            return result;
        }

        public static DiaperSizerResponseDto GetFailedResponse()
        {
            DiaperSizerResponseDto result = new DiaperSizerResponseDto();
            result.Result = ErrorCodes.Failed;
            result.Exception = ErrorDescription.Failed;
            return result;
        }

        public static DiaperSizerResponseDto GetUnexpectedErrorResponse()
        {
            DiaperSizerResponseDto result = new DiaperSizerResponseDto();
            result.Result = ErrorCodes.UnexpectedError;
            result.Exception = ErrorDescription.UnexpectedError;
            return result;
        }

        public static DiaperSizerResponseDto GetNoResultsFoundResponse()
        {
            DiaperSizerResponseDto result = new DiaperSizerResponseDto();
            result.Result = ErrorCodes.NoResultsFound;
            result.Exception = ErrorDescription.NoResultsFound;
            return result;
        }

        public static DiaperSizerResponseDto GetBadParametersResponse()
        {
            DiaperSizerResponseDto result = new DiaperSizerResponseDto();
            result.Result = ErrorCodes.BadParameters;
            result.Exception = ErrorDescription.BadParameters;
            return result;
        }
    }
}
