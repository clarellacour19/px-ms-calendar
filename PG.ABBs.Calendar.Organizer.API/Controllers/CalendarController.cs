using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PG.ABBs.Calendar.Organizer.Service.Dto;
using PG.ABBs.Calendar.Organizer.Service.Helper;
using PG.ABBs.Calendar.Organizer.Service.Services;

namespace PG.ABBs.Calendar.Organizer.API.Controllers
{
	/// </summary>
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	[Produces("application/json")]
	[ApiController]
	public class CalendarController : Controller
	{
		private readonly ICalendarService calendarService;
		private readonly ILogger logger;
		private readonly TelemetryClient telemetryClient;

		public CalendarController(ICalendarService calendarService, ILogger<CalendarController> loggerProvider, TelemetryClient telemetryClient)
		{
			this.calendarService = calendarService;
			this.logger = loggerProvider;
			this.telemetryClient = telemetryClient;
		}


		[HttpPost]
		[Route("BatchUpdateCalendar")]
		public IActionResult BatchUpdateCalendar([FromBody] BatchUpdateCalendarDto Dto)
		{
			var apiResponse = new ApiResponse();
			try
			{
				Response.OnCompleted(async () => { await this.calendarService.BatchUpdateCalendar(Dto); });
				apiResponse.UpdateResult(Constants.ErrorCodes.Ok, null);
			}
			catch (Exception ex)
			{
				apiResponse.UpdateResultWithException(Constants.ErrorCodes.Failed, ex);
				this.logger.LogError(
					$"Error during BatchUpdateCalendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return this.Json(apiResponse);
		}

		[HttpPost]
		[Route("GenerateCalendar")]
		public IActionResult GenerateCalendar([FromBody] GenerateCalendarDto Dto)
		{	
			var apiResponse = new ApiResponse();
			try
			{
				var apiName = "GenerateCalendar";
				var message =$"Generate Method Step 1 at {DateTime.UtcNow.ToString()}";
				ApplicationInsightsHelper.SendCustomLog(this.telemetryClient,message, apiName, apiName, apiName);
				var fromObject = this.calendarService.GenerateCalendar(Dto);
				apiResponse.UpdateResult(Constants.ErrorCodes.Ok, fromObject);
			}
			catch (Exception ex)
			{
				apiResponse.UpdateResultWithException(Constants.ErrorCodes.Failed, ex);
				this.logger.LogError(
					$"Error during GenerateCalendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return this.Json(apiResponse);
		}

		[HttpPost]
		[Route("GetUserCalendar")]
		public IActionResult GetUserCalendar([FromBody] GetUserCalendarDto Dto)
		{
			var apiResponse = new ApiResponse();
			try
			{
				var apiName = "GetUserCalendar";
				var message = $"GetUserCalendar Method Step 1 at {DateTime.UtcNow.ToString()}";
				this.telemetryClient.TrackTrace("Test",SeverityLevel.Warning);
				ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);

				var fromObject = this.calendarService.GetUserCalendar(Dto);
				apiResponse.UpdateResult(Constants.ErrorCodes.Ok, fromObject);
			}
			catch (Exception ex)
			{
				apiResponse.UpdateResultWithException(Constants.ErrorCodes.Failed, ex);
				this.logger.LogError(
					$"Error during GetUserCalendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return this.Json(apiResponse);
		}
	}
}