﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentful.Core.Extensions;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NodaTime;
using PG.ABBs.Calendar.Organizer.AzureStorage;
using PG.ABBs.Calendar.Organizer.Content;
using PG.ABBs.Calendar.Organizer.Content.Configuration;
using PG.ABBs.Calendar.Organizer.Content.Domain;
using PG.ABBs.Calendar.Organizer.Content.Repository;
using PG.ABBs.Calendar.Organizer.Data.Constant;
using PG.ABBs.Calendar.Organizer.Data.Context;
using PG.ABBs.Calendar.Organizer.Data.Models;
using PG.ABBs.Calendar.Organizer.Data.Repositories;
using PG.ABBs.Calendar.Organizer.Service.Dto;
using PG.ABBs.Calendar.Organizer.Service.Helper;
using CalendarEvent = PG.ABBs.Calendar.Organizer.Content.Domain.CalendarEvent;

namespace PG.ABBs.Calendar.Organizer.Service.Services
{
	public class CalendarService : ICalendarService
	{
		private readonly IUnitOfWork<DataContext> unitOfWork;

		private readonly IOptions<List<MarketSettings>> marketSettings;
		private readonly IOptions<StorageModel> azureStorage;


		private readonly ContentManager contentManager;


		private readonly ILogger logger;

		private readonly StorageClient storageClient;
		private readonly TelemetryClient telemetryClient;

		public CalendarService(
			IUnitOfWork<DataContext> unitOfWork,
			ContentManager contentManager,
			IOptions<StorageModel> azureStorage,
			IOptions<List<MarketSettings>> marketSettings,
			IOptions<List<string>> contentTypeSettings,
			MarketSettingsHelper marketSettingsHelper,
			TelemetryClient telemetryClient,
			StorageClient storageClient,
			ILogger<CalendarService> loggerProvider,
			IConfiguration configuration)
		{
			this.unitOfWork = unitOfWork;
			this.contentManager = contentManager;
			this.marketSettings = marketSettings;
			this.azureStorage = azureStorage;
			this.storageClient = storageClient;
			this.telemetryClient = telemetryClient;
			this.logger = loggerProvider;

		}

		public async Task<List<string>> BatchUpdateCalendar(BatchUpdateCalendarDto Dto)
		{
			string site, locale = null;
			if (!ReferenceEquals(Dto, null))
			{
				
				locale = Dto.locale;
			}

			var beginTime = DateTime.UtcNow;
			DateTime endTime;
			this.logger.LogWarning($"Update Calendar Batch begin : {beginTime}");
			var errorList = new List<String>();

			try
			{
				var repository = this.contentManager.GetRepository();


				if (ReferenceEquals(locale, null))
				{
					//use threading Parallel	
					Parallel.ForEach(marketSettings.Value,
						market => { errorList.AddRange(ProcessBatch(market, repository)?.Result); });
				}
				else
				{
					var market = marketSettings.Value.Where(m => m.Language.Equals(locale));

					if (!ReferenceEquals(market.First(), null))
					{
						errorList.AddRange(ProcessBatch(market.First(), repository)?.Result);
					}
				}
			}
			catch (System.Exception ex)
			{
				errorList.Add($"Error during process: {ex.Message} - {DateTime.UtcNow}");
				this.logger.LogError($"Error during process: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}
			finally
			{
				endTime = DateTime.UtcNow;
				this.logger.LogWarning($"Update Calendar Batch End : {endTime}");
			}

			var time = endTime.Subtract(beginTime);


			var result = new List<string>
			{
				$"BeginTime: {beginTime}",
				$"EndTime: {endTime}",
				$"ProcessTimeTook:" + $"{time.Hours}h:{time.Minutes}mm:{time.Seconds}s:{time.Milliseconds}ms",
				$"Errors: {errorList}",
			};

			return result;
		}

		public CalendarDto GenerateCalendar(GenerateCalendarDto Dto)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var errorList = new List<String>();
			string site, locale, uuidHash, dueDate = null;
			if (ReferenceEquals(Dto, null))
			{
				this.logger.LogWarning($"GenerateCalendar DTO is empty- No Calendars generated");
				return null;
			}
			else
			{
				locale = Dto.locale;
				uuidHash = Dto.uuidHash;
				dueDate = Dto.dueDate;
			}

			try
			{
				//var message = new string($"Generate Method Step 2 at {DateTime.UtcNow.ToString()}");
				var apiName = "GenerateCalendar";
				
				var dueDateHash = OrganizerHelper.CreateMD5(dueDate.ToString());
				 //message = new string($"Generate Method Step 3 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 3");

				var dueDateParsed = new DateTime();
				DateTime.TryParse(dueDate, out dueDateParsed);
				var market = marketSettings.Value.Where(m => m.Language.Equals(locale)).First();

				//message = new string($"Generate Method Step 3 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 4");

				CalendarDto calendarObj = new CalendarDto();

				var argsToGetDueDateHash = new Dictionary<string, object>
				{
					{ "locale", locale },
					{ "dueDateHash", dueDateHash },
					{ "limit", "" },
					{ "sorting", "" },
				};

				var listOfCalendars = this.unitOfWork.GetRepository<Data.Models.Calendar>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetCalendars,
						argsToGetDueDateHash); //LOCALE

				//message = new string($"Generate Method Step 4 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 5");

				if (!listOfCalendars.Any())
				{
					//generate new calender amd upload to storage
					var calendar = GenerateCalender(dueDateParsed, locale, dueDateHash,market);

					//message = new string($"Generate Method Step 5 at {DateTime.UtcNow.ToString()}");
					//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
					ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 6");

					this.storageClient.UploadCalendar(calendar, locale,dueDateHash);

					//message = new string($"Generate Method Step 6 at {DateTime.UtcNow.ToString()}");
					//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
					ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 7");

					//add to db

					AddOrUpdateCalendar<Data.Models.Calendar>(new Data.Models.Calendar()
					{
						UuidHash = uuidHash,
						DueDate = dueDateParsed,
						DueDateHash = dueDateHash,
						DateCreated = DateTime.UtcNow,
						Locale = locale
					});

					calendarObj = new CalendarDto
					{
						
						UuidHash = uuidHash,
						DueDate = dueDateParsed,
						DueDateHash = dueDateHash,
						DateCreated = DateTime.UtcNow,
						Locale = locale,
						CdnUrl = $"{azureStorage.Value.CdnPrefix}/{market.Language}/{dueDateHash}.ics"

					};

					//message = new string($"Generate Method Step 7 at {DateTime.UtcNow.ToString()}");
					//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
					ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 8");


				}
				else
				{
					//add user calender
					var argsToAddUserCalender = new Dictionary<string, object>
					{
						{ "uuidHash", uuidHash },
						{ "dueDateHash", listOfCalendars.First().DueDateHash },
						{ "calenderId", listOfCalendars.First().CalendarId },
						{ "locale", listOfCalendars.First().Locale }
					};

					this.unitOfWork.GetRepository<UserCalendar>().ExecuteNonQueryStoredProcedure(
						Constant.DatabaseObject.StoredProcedure.AddOrUpdateUserCalendar, argsToAddUserCalender);

					//message = new string($"Generate Method Step 8 at {DateTime.UtcNow.ToString()}");
					//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
					ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GenerateCalendar", "GenerateCalendar Step 9");


					calendarObj = new CalendarDto
					{
						CalendarId = listOfCalendars.First().CalendarId,
						UuidHash = uuidHash,
						DueDate = dueDateParsed,
						DueDateHash = dueDateHash,
						DateCreated = DateTime.UtcNow,
						Locale = locale,
						CdnUrl = $"{azureStorage.Value.CdnPrefix}/{market.Language}/{dueDateHash}.ics"

					};
				}

				return calendarObj;
			}
			catch (System.Exception ex)
			{
				this.logger.LogError(
					$"Error during  generate calendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace} for market {locale}");
				errorList.Add($"Error during generate calendar: {ex.Message} - {DateTime.UtcNow} for market {locale}");
				throw;
			}

			
		}

		public ReturnGetUserCalendarDto GetUserCalendar(GetUserCalendarDto Dto)
		{
			string site, locale, uuidHash, sorting = null;
			int? limit;
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			if (ReferenceEquals(Dto, null))
			{
				this.logger.LogWarning($"GetUserCalendar DTO is empty- No Calendars retrieved");
				return null;
			}
			else
			{
				locale = Dto.locale;
				uuidHash = Dto.uuidHash;
				sorting = Dto.sorting;
				limit = Dto.limit;
			}

			try
			{
				var apiName = "GetUserCalendar";

				var market = marketSettings.Value.Where(m => m.Language.Equals(locale));

				//var message = new string($"GetUserCalendar Method Step 2 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GetUserCalendar", "GetUserCalendar Step 2");



				var argsToGetDueDateHash = new Dictionary<string, object>
				{
					{ "locale", locale },
					{ "uuidHash", uuidHash },
					{ "limit", limit },
					{ "sorting", sorting },
				};
				var listOfCalendars = this.unitOfWork.GetRepository<Data.Models.Calendar>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetUserCalendars,
						argsToGetDueDateHash).ToList();
				//message = new string($"GetUserCalendar Method Step 3 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GetUserCalendar", "GetUserCalendar Step 3");


				var calendarDto = new List<CalendarDto>();

				foreach (var item in listOfCalendars)
				{
					calendarDto.Add(new CalendarDto
					{
						CalendarId = item.CalendarId,
						DateCreated = item.DateCreated,
						DueDate = item.DueDate,
						UuidHash = item.UuidHash,
						DueDateHash = item.DueDateHash,
						CdnUrl = $"{azureStorage.Value.CdnPrefix}/{market.First().Language}/{item.DueDateHash}.ics",
						Locale = item.Locale
					});
				}

				//message = new string($"GetUserCalendar Method Step 4 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GetUserCalendar", "GetUserCalendar Step 4");


				var returnGetUserCalendarDto = new ReturnGetUserCalendarDto
				{
					Calendar = calendarDto
				};
				//message = new string($"GetUserCalendar Method Step 5 at {DateTime.UtcNow.ToString()}");
				//ApplicationInsightsHelper.SendCustomLog(this.telemetryClient, message, apiName, apiName, apiName);
				ApplicationInsightsHelper.SendEventTracking(this.telemetryClient, stopwatch, apiName, "CalendarService", "GetUserCalendar", "GetUserCalendar Step 5");


				return returnGetUserCalendarDto;
			}
			catch (System.Exception ex)
			{
				this.logger.LogError(
					$"Error during GetUserCalendar: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
				throw;
			}
		}

		protected async Task<List<string>> ProcessBatch(MarketSettings market, IContentRepository repository)
		{
			this.logger.LogWarning($"Update Calendar Batch Started for : {market.Language} at {DateTime.UtcNow}");

			var errorList = new List<string>();
			var wasUpdated = false;

			try
			{
				var listOfCalendarEvents =
					(await repository.GetAll<CalendarEvent>(market.Language, ContentSettings.CalendarEvent))
					.ToList();


				var argsLocale = new Dictionary<string, object>
				{
					{ "locale", market.Language }
				};

				var listOfEventsOnDb = this.unitOfWork.GetRepository<Events>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetEvents, argsLocale);

				foreach (var events in listOfEventsOnDb)
				{
					var presentOnContentful =
						listOfCalendarEvents.Where(e => e.Sys.Id.Equals(events.ContentId)).ToList();
					if (!presentOnContentful.Any())
					{
						var argsToGet = new Dictionary<string, object>
						{
							{ "contentId", events.ContentId }
						};

						this.unitOfWork.GetRepository<Events>()
							.ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.DeleteEvent,
								argsToGet);
						this.logger.LogInformation(
							$"Batch deleted one obsolete event on DB : {events.ContentId} for market {market.Language}");
					}
					else
					{
						if (
							presentOnContentful.First().Sys.PublishedAt > events.LastCreated
						)
						{
							AddOrUpdateEvent<CalendarEvent>(presentOnContentful.First());
							listOfCalendarEvents.Remove(presentOnContentful.First());
							wasUpdated = true;
						}
					}
				}

				//new events not present in db
				foreach (var calendarEvent in listOfCalendarEvents)
				{
					if (!listOfEventsOnDb.Any(db => db.ContentId.Equals(calendarEvent.Sys.Id)))
					{
						calendarEvent.Locale = market.Language;
						AddOrUpdateEvent<Events>(calendarEvent);
						wasUpdated = true;
					}
				}


				TimeSpan span = DateTime.UtcNow.AddMonths(market.DeleteTimeSpan) - DateTime.UtcNow;


				var fullListOfCalendars = this.unitOfWork.GetRepository<Data.Models.Calendar>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetAllCalendars, argsLocale)
					.ToList(); //LOCALE
				for (int i = 0; i < fullListOfCalendars.Count; i++)
				{
					var calendar = fullListOfCalendars[i];
					if (DateTime.UtcNow - calendar.DueDate > span)
					{
						var argsDeleteCalendar = new Dictionary<string, object>
						{
							{ "dueDateHash", calendar.DueDateHash },
							{ "locale", market.Language }
						};
						//delete
						this.unitOfWork.GetRepository<Data.Models.Calendar>()
							.ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.DeleteCalendar,
								argsDeleteCalendar);
						this.storageClient.DeleteCalendar($"{calendar.DueDateHash}", market.Language);
						this.logger.LogInformation(
							$"Batch deleted one obsolete Calendar on DB and Azure storage: {calendar.DueDateHash} for market {market.Language}");
						//remove
						fullListOfCalendars.Remove(calendar);
					}
				}

				//var listOfCalsToDel = new List<string>();
				//var listOfGeneratedCals = new List<Ical.Net.Calendar>();

				if (wasUpdated)
				{
					//get all calendar
					// run process to generate new ICS


					if (!ReferenceEquals(fullListOfCalendars, null))
					{
						var listOfCalendars = fullListOfCalendars;
						foreach (var calendar in listOfCalendars)
						{
							//GENERATE NEW CALENDER

							var newCalender = GenerateCalender(calendar.DueDate, market.Language, calendar.DueDateHash,market);
							//listOfGeneratedCals.Add(newCalender);
							//listOfCalsToDel.Add(newCalender.Name);
							this.storageClient.DeleteCalendar($"{calendar.DueDateHash}", market.Language);
							this.storageClient.UploadCalendar(newCalender, market.Language, calendar.DueDateHash);
							//update db

							AddOrUpdateCalendar<Data.Models.Calendar>(new Data.Models.Calendar()
							{
								UuidHash = calendar.UuidHash,
								DueDate = calendar.DueDate,
								DueDateHash = calendar.DueDateHash,
								DateCreated = DateTime.UtcNow,
								Locale = market.Language
							});
							this.logger.LogInformation(
								$"Batch generated a new Calendar on DB and on Azure storage: {calendar.DueDateHash} for market {market.Language}");
						}
					}
				}

				//upload all async
				//if (listOfCalsToDel.Any())
				//{
				//	await this.storageClient.DeleteCalendarAsync(market.Language, listOfCalsToDel);
				//	this.logger.LogInformation($"Batch Deleted the following Calendars for {listOfCalsToDel.ToArray().ToString()} for market {market.Language}");
				//}

				////upload all async
				//if (listOfGeneratedCals.Any())
				//{
				//	await this.storageClient.UploadCalendarsAsync(market.Language, listOfGeneratedCals);
				//	this.logger.LogInformation($"Batch Uploaded the following Calendars for {listOfGeneratedCals.ToArray().ToString()} for market {market.Language}");

				//}
				this.logger.LogWarning($"Update Calendar Batch Ended for : {market.Language} at {DateTime.UtcNow}");
			}
			catch (System.Exception ex)
			{
				errorList.Add(
					$"Error during Update Calendar Batch process : {market.Language} - {ex.Message} - {DateTime.UtcNow}");
				this.logger.LogError(
					$"Error during Update Calendar Batch process : {market.Language} - {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return errorList;
		}

		public void AddOrUpdateEvent<T>(CalendarEvent obj) where T : class
		{
			var argsToGet = new Dictionary<string, object>
			{
				{ "contentId", obj.Sys.Id },
				{ "lastCreated", DateTime.UtcNow },
				{ "title", obj.Title },
				{ "description", obj.Description },
				{ "period", obj.Period },
				{ "number", obj.Number },
				{ "url", obj.Url },
				{ "type", obj.Type },
				{ "locale", obj.Locale }
			}; //ADD OR UPDATE

			this.unitOfWork.GetRepository<T>()
				.ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.AddOrUpdateEvents, argsToGet);
		}

		public void AddOrUpdateCalendar<T>(Data.Models.Calendar obj) where T : class
		{
			var argsToGet = new Dictionary<string, object>
			{
				{ "UuidHash", obj.UuidHash },
				{ "dueDate", obj.DueDate },
				{ "dueDateHash", obj.DueDateHash },
				{ "dateCreated", obj.DateCreated },
				{ "locale", obj.Locale },
			}; //ADD OR UPDATE

			this.unitOfWork.GetRepository<T>()
				.ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.AddOrUpdateCalendars,
					argsToGet);
		}

		public Ical.Net.Calendar GenerateCalender(DateTime dueDate, string locale, string dueDateHash, MarketSettings market)
		{
			//new calender initilized
			var calendar = new Ical.Net.Calendar();

			calendar.Name = "VCALENDAR";//dueDateHash.ToString();
			calendar.Properties.Add(new CalendarProperty("X-WR-CALNAME", "Pampers Calendar"));
			calendar.Properties.Add(new CalendarProperty("NAME", "Pampers prenatal Calendar"));
			calendar.Version = "2.0";
			calendar.ProductId = $"-//{market.DomainName}//Calendar Organizer 1.0//EN";
			calendar.AddTimeZone($"{market.TimeZone}");
			var alarm = new Alarm
			{
				Action = "DISPLAY",
				Description = "PAMPERS 💛 Your Personal Pregnancy Guide",
				Trigger = new Trigger(TimeSpan.FromHours(-10))
			};

			calendar.Properties.Add(new CalendarProperty("REFRESH-INTERVAL;VALUE=DURATION", "P12H"));

			//get list of events for that locale
			var argsToGetall = new Dictionary<string, object>
			{
				{ "locale", locale }
			};

			var listOfEventsOnDb = this.unitOfWork.GetRepository<Events>()
				.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetEvents, argsToGetall); //LOCALE

			//add each events wrt to properties

			foreach (var events in listOfEventsOnDb)
			{
				DateTime startTime;
				if (events.Type.Equals("prenatal"))
				{
					startTime = (dueDate.AddDays(-266));
				}
				else //postnatal
				{
					startTime = dueDate;
				}

				switch (events.Period)
				{
					case "day":
						startTime = startTime.AddDays(events.Number);
						break;
					case "week":
						startTime = startTime.AddDays(7 * events.Number);
						break;
					case "month":
						startTime = startTime.AddMonths(events.Number);
						break;
					case "year":
						startTime = startTime.AddYears(events.Number);
						break;
				}

				int year, month, day, hour, minute, second;
				Int32.TryParse(startTime.Year.ToString(), out year);
				Int32.TryParse(startTime.Month.ToString(), out month);
				Int32.TryParse(startTime.Day.ToString(), out day); ;
				Int32.TryParse(startTime.Minute.ToString(), out minute);
				Int32.TryParse(startTime.Second.ToString(), out second);
				var date = new CalDateTime(year, month, day, market.StartTime??12, minute, second);
				var iCalEvent = new Ical.Net.CalendarComponents.CalendarEvent
				{	
					DtStamp = new CalDateTime(DateTime.UtcNow),
					Uid = $"{events.ContentId}-{locale}@{market.DomainName}",
					DtStart = date,
					DtEnd = date,
					Summary = events.Title,
					Description = $"{events.Description}\n\n{events.URL}",//events.Description,
					Start = date,
					IsAllDay = false,
					Location = events.URL,
					Alarms =
					{
						alarm
					}


				};

				calendar.Events.Add(iCalEvent);
			}

			return calendar;
		}

		
	}
}