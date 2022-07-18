using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
		private readonly List<JanrainProvider> _janrainProviders;
		private readonly string _encryptionV2Key;
		private readonly string _ivvar;

		public CalendarController(ICalendarService calendarService,
			ILogger<CalendarController> loggerProvider,
			TelemetryClient telemetryClient, 
			IConfiguration configuration,
			IOptions<List<JanrainProvider>> janrainProviders)
		{
			this.calendarService = calendarService;
			this.logger = loggerProvider;
			this.telemetryClient = telemetryClient;
			this._janrainProviders = janrainProviders.Value;
			this._ivvar = configuration["IvVariable"];
			this._encryptionV2Key = configuration["EncryptionV2Key"];
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
		public async Task<IActionResult> GenerateCalendar([FromBody] GenerateCalendarDto Dto)
		{	
			var apiResponse = new ApiResponse();
			try
			{
				if (!string.IsNullOrEmpty(Dto.AccessToken)) // to remove when all FE matches call
				{
					Dto.AccessToken = Uri.UnescapeDataString(Dto.AccessToken);
					if (!await ProviderHelper.VerifyProfile(this._janrainProviders,
						this._encryptionV2Key,
						this._ivvar,
						Dto.ConsumerId,
						Dto.AccessToken,
						Dto.locale))
					{
						apiResponse.UpdateResult(Constants.ErrorCodes.BadParameters, "access token invalid");
						this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
						return this.Json(apiResponse);
					}
				}
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var apiName = "GenerateCalendar";
				var message =$"Generate Method Step 1 at {DateTime.UtcNow.ToString()}";
				this.logger.LogInformation(message+"from logger");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient,message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarController","GenerateCalendar","GenerateCalendar");
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
		[Route("TestCalendar")]
		public async Task<IActionResult> TestCalendar([FromBody] GenerateCalendarDto Dto)
		{
			var apiResponse = new ApiResponse();
			try
			{
				if (!string.IsNullOrEmpty(Dto.AccessToken)) // to remove when all FE matches call
				{
					Dto.AccessToken = Uri.UnescapeDataString(Dto.AccessToken);
					if (!await ProviderHelper.VerifyProfile(this._janrainProviders,
						this._encryptionV2Key,
						this._ivvar,
						Dto.ConsumerId,
						Dto.AccessToken,
						Dto.locale))
					{
						apiResponse.UpdateResult(Constants.ErrorCodes.BadParameters, "access token invalid");
						this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
						return this.Json(apiResponse);
					}
				}
				var apiName = "TestCalendar";
				var message = $"Generate Method Step 1 at {DateTime.UtcNow.ToString()}";
				ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				var fromObject = this.calendarService.GenerateCalendar(Dto);
				apiResponse.UpdateResult(Constants.ErrorCodes.Ok, fromObject);
			}
			catch (Exception ex)
			{
				apiResponse.UpdateResultWithException(Constants.ErrorCodes.Failed, ex);
				this.logger.LogError(
					$"Error during TestCalendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return this.Json(apiResponse);
		}

		[HttpPost]
		[Route("GetUserCalendar")]
		public async Task<IActionResult> GetUserCalendar([FromBody] GetUserCalendarDto Dto)
		{
			var apiResponse = new ApiResponse();
			try
			{
				if (!string.IsNullOrEmpty(Dto.AccessToken)) // to remove when all FE matches call
				{
					Dto.AccessToken = Uri.UnescapeDataString(Dto.AccessToken);
					if (!await ProviderHelper.VerifyProfile(this._janrainProviders,
						this._encryptionV2Key,
						this._ivvar,
						Dto.ConsumerId,
						Dto.AccessToken,
						Dto.locale))
					{
						apiResponse.UpdateResult(Constants.ErrorCodes.BadParameters, "access token invalid");
						this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
						return this.Json(apiResponse);
					}
				}
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var apiName = "GetUserCalendar";
				//var message = $"GetUserCalendar Method Step 1 at {DateTime.UtcNow.ToString()}";
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarController", "GetUserCalendar", "GetUserCalendar Step 1");

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