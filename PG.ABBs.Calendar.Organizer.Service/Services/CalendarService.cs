using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

		private readonly IOptions<SharedMarketSettings> sharedMarketSettings;

		private readonly IOptions<List<string>> contentTypeSettings;

		private readonly ContentManager contentManager;

		private readonly MarketSettingsHelper marketSettingsHelper;

		private readonly ILogger logger;

		private readonly StorageClient storageClient;

		public CalendarService(
			IUnitOfWork<DataContext> unitOfWork,
			ContentManager contentManager,
			IOptions<List<MarketSettings>> marketSettings,
			IOptions<SharedMarketSettings> sharedMarketSettings,
			IOptions<List<string>> contentTypeSettings,
			MarketSettingsHelper marketSettingsHelper,
			StorageClient storageClient,
			ILogger<CalendarService> loggerProvider)
		{
			this.unitOfWork = unitOfWork;
			this.contentManager = contentManager;
			this.marketSettings = marketSettings;
			this.sharedMarketSettings = sharedMarketSettings;
			this.contentTypeSettings = contentTypeSettings;
			this.storageClient = storageClient;
			this.marketSettingsHelper = marketSettingsHelper;
			this.logger = loggerProvider;
		}

		public async Task<object> BatchUpdateCalendar(BatchUpdateCalendarDto Dto)
		{
			string site, locale = null;
			if (!ReferenceEquals(Dto, null))
			{
				site = Dto.site;
				locale = Dto.locale;
			}

			var beginTime = DateTime.UtcNow;
			DateTime endTime;
			this.logger.LogWarning($"Update Calendar Batch begin : {beginTime}");
			var errorList = new List<String>();

			try
			{
				var repository = this.contentManager.GetRepository();
				//use threading Parallel
				Parallel.ForEach(marketSettings.Value, market =>
				{
					errorList.AddRange(ProcessBatch(market, repository)?.Result);

				});


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

			var result = new
			{
				BeginTime = beginTime,
				EndTime = endTime,
				ProcessTimeTook = $"{time.Hours}h:{time.Minutes}mm:{time.Seconds}s:{time.Milliseconds}ms",
				Errors = errorList
			};

			return result;
		}

		public object GenerateCalendar(GenerateCalendarDto Dto)
		{
			var errorList = new List<String>();
			string site, locale, uuidHash, dueDate = null;
			if (ReferenceEquals(Dto, null))
			{	
				//TODO: add logger
				return null;
			}
			else
			{
				site = Dto.site;
				locale = Dto.locale;
				uuidHash = Dto.uuidHash;
				dueDate = Dto.dueDate;
			}

			try
			{
				var dueDateHash = OrganizerHelper.CreateMD5(dueDate);
				var dueDateParsed = DateTime.Parse(dueDate);

				//var argsGetCount = new Dictionary<string, object>
				//{
				//	{ "table", "Calendar" },
				//	{ "locale", locale }
				//};

				//var getCalCount = this.unitOfWork.GetRepository<GetCountInTable>()
				//	.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetCountInTable, argsGetCount);


				var argsToGetDueDateHash = new Dictionary<string, object>
				{
					{ "locale", locale },
					{ "dueDateHash", dueDateHash },
					{ "limit", "" },
					{ "sorting", "" },

				};

				var listOfCalendars = this.unitOfWork.GetRepository<Data.Models.Calendar>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetCalendars, argsToGetDueDateHash);//LOCALE
				if (!listOfCalendars.Any())
				{
					//generate new calender amd upload to storage
					var calendar = GenerateCalender(dueDateParsed, locale);

					this.storageClient.UploadCalendar(calendar);

					//add to db
					
					AddOrUpdateCalendar<Data.Models.Calendar>(new Data.Models.Calendar()
					{
						UuidHash = uuidHash,
						DueDate = dueDateParsed,
						DueDateHash = dueDateHash,
						DateCreated = DateTime.UtcNow,
						Locale = locale
					});
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

					this.unitOfWork.GetRepository<UserCalendar>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.AddOrUpdateUserCalendar, argsToAddUserCalender);

				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex);//TODO: add logger
				errorList.Add($"Error during generate calendar: {ex.Message} - {DateTime.UtcNow}");
			}
			
			return errorList;
		}

		public Task<List<Data.Models.Calendar>> GetUserCalendar(GetUserCalendarDto Dto)
		{
			string site, locale, uuidHash, sorting = null;
			int? limit;

			if (ReferenceEquals(Dto, null))
			{
				//TODO: add logger
				return null;
			}
			else
			{
				site = Dto.site;
				locale = Dto.locale;
				uuidHash = Dto.uuidHash;
				sorting = Dto.sorting;
				limit = Dto.limit;
			}

			try
			{
				var argsToGetDueDateHash = new Dictionary<string, object>
				{
					{ "locale", locale },
					{ "uuidHash", uuidHash },
					{ "limit", limit },
					{ "sorting", sorting },
				

				};
				var listOfCalendars = this.unitOfWork.GetRepository<Data.Models.Calendar>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetUserCalendars, argsToGetDueDateHash); //TO UPDATE
				return (Task<List<Data.Models.Calendar>>)listOfCalendars;
			}
			catch (System.Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		protected async Task<List<string>> ProcessBatch(MarketSettings market, IContentRepository repository)
		{
			var errorList = new List<string>();
			var wasUpdated = false;

			try
			{
				//get domain name as well 
				var siteSettings =
					(await repository.GetAll<SiteSettingsContent>(market.Language, ContentSettings.SiteSetting))
					.ToList();
				var domainUrl = siteSettings.Any()
					? siteSettings[0].Domain
					: string.Empty;
				var listOfCalendarEvents = (await repository.GetAll<CalendarEvent>(market.Language, ContentSettings.CalendarEvent))
					.ToList();


				var argsToGetall = new Dictionary<string, object>
				{
					{ "locale", market.Language }
				};

				var listOfEventsOnDb = this.unitOfWork.GetRepository<Events>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetEvents, argsToGetall);//LOCALE

				foreach (var events in listOfEventsOnDb)
				{
					var presentOnContentful = listOfCalendarEvents.Where(e => e.Sys.Id.Equals(events.ContentId)).ToList();
					if (!presentOnContentful.Any())
					{
						var argsToGet = new Dictionary<string, object>
						{
							{ "contentId", events.ContentId }
						};

						this.unitOfWork.GetRepository<Events>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.DeleteEvent, argsToGet);
						this.logger.LogInformation($"Batch deleted one obsolete event on DB : {events.ContentId}");
					}
					else
					{
						if (
							presentOnContentful.First().Sys.PublishedAt>events.LastCreated

						   )
						{
							AddOrUpdateEvent<CalendarEvent>(presentOnContentful.First());
							listOfCalendarEvents.Remove(presentOnContentful.First());
							wasUpdated = true;
						}
						else
						{
							
						}
					}
				}

				//new events not present in db
				foreach (var calendarEvent in listOfCalendarEvents)
				{
					calendarEvent.Locale = market.Language;
					AddOrUpdateEvent<Events>(calendarEvent);
					wasUpdated = true;
				}

				var argsGetCount = new Dictionary<string, object>
				{
					{ "table", "Calendar" },
					{ "locale", market.Language }
				};
				var argsLocale = new Dictionary<string, object>
				{
					
					{ "locale", market.Language }
				};

				TimeSpan span = DateTime.UtcNow.AddYears(market.DeleteTimeSpan) - DateTime.UtcNow;

				var getCalCount = this.unitOfWork.GetRepository<GetCountInTable>()
					.ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetCountInTable, argsGetCount);
				List<Data.Models.Calendar> fullListOfCalendars = null;

				if (getCalCount.First().Count > 0)
				{
					fullListOfCalendars = (List<Data.Models.Calendar>)this.unitOfWork.GetRepository<Data.Models.Calendar>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetAllCalendars, argsLocale);//LOCALE
					for (int i = 0; i < fullListOfCalendars.Count; i++)
					{
						var calendar = fullListOfCalendars[i];
						if (DateTime.UtcNow - calendar.DateCreated > span)
						{
							var argsDeleteCalendar = new Dictionary<string, object>
							{
								{ "dueDateHash", calendar.DueDateHash },
								{ "locale", market.Language }
							};
							//delete
							this.unitOfWork.GetRepository<Data.Models.Calendar>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.DeleteCalendar, argsDeleteCalendar);
							this.storageClient.DeleteCalendar($"{calendar.DueDateHash}.ics");
							this.logger.LogInformation($"Batch deleted one obsolete Calendar on DB and Azure storage: {calendar.DueDateHash}");
							//remove
							fullListOfCalendars.Remove(calendar);
						}
					}
				}

				//fullListOfCalendars = (List<Data.Models.Calendar>)this.unitOfWork.GetRepository<Data.Models.Calendar>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetAllCalendars, argsLocale);//LOCALE

				

				

				/////
				if (wasUpdated)
				{
					//get all calendar
					// run process to generate new ICS

					//var argsLocale = new Dictionary<string, object>
					//{
					//	{ "locale", market.Language }
					//};

					var listOfEvents = this.unitOfWork.GetRepository<Events>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetEvents, argsLocale);//LOCALE

					if (!ReferenceEquals(fullListOfCalendars,null))
					{
						var listOfCalendars = fullListOfCalendars;//this.unitOfWork.GetRepository<Data.Models.Calendar>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetCalendars, argsLocale);//LOCALE
						foreach (var calendar in listOfCalendars)
						{
							//GENERATE NEW CALENDER

							var newCalender = GenerateCalender(calendar.DueDate, market.Language);
							this.storageClient.DeleteCalendar(newCalender.Name);
							this.storageClient.UploadCalendar(newCalender);

							//update db

							AddOrUpdateCalendar<Data.Models.Calendar>(new Data.Models.Calendar()
							{
								UuidHash = calendar.UuidHash,
								DueDate = calendar.DueDate,
								DueDateHash = calendar.DueDateHash,
								DateCreated = DateTime.UtcNow,
								Locale = market.Language
							});

						}
					}

					

				}

			}
			catch (System.Exception ex)
			{
				errorList.Add($"Error during Update Calendar Batch process : {market.Language} - {ex.Message} - {DateTime.UtcNow}");
				this.logger.LogError($"Error during Update Calendar Batch process : {market.Language} - {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
			}

			return errorList;
		}

		public void AddOrUpdateEvent<T>(CalendarEvent obj) where T:class
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
				{"type",obj.Type},
				{"locale",obj.Locale}
			};//ADD OR UPDATE

			this.unitOfWork.GetRepository<T>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.AddOrUpdateEvents, argsToGet);
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

				
			};//ADD OR UPDATE

			this.unitOfWork.GetRepository<T>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.AddOrUpdateCalendars, argsToGet);
		}

		public Ical.Net.Calendar GenerateCalender(DateTime dueDate, string locale)
		{
			//new calender initilized
			var calendar = new Ical.Net.Calendar()
			{
				Name = OrganizerHelper.CreateMD5(dueDate.ToString())
			};

			
			//get list of events for that locale
			 var argsToGetall = new Dictionary<string, object>
			{
				{ "locale", locale }
			};

			var listOfEventsOnDb = this.unitOfWork.GetRepository<Events>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetEvents, argsToGetall);//LOCALE

			//add each events wrt to properties

			foreach (var events in listOfEventsOnDb)
			{	
				DateTime startTime;
				if (events.Type.Equals("prenatal"))
				{
					startTime = (dueDate.AddDays(-266));

					
					
				}
				else//postnatal
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
				var iCalEvent = new Ical.Net.CalendarComponents.CalendarEvent
				{	
					Summary = events.Title,
					Description = events.Description,
					Start =  new CalDateTime(Int32.Parse(startTime.Year.ToString()), Int32.Parse(startTime.Month.ToString()), Int32.Parse(startTime.Day.ToString()), Int32.Parse(startTime.Hour.ToString()), Int32.Parse(startTime.Minute.ToString()), Int32.Parse(startTime.Second.ToString())),
					IsAllDay = true,
					Location = events.URL
				};

				calendar.Events.Add(iCalEvent);
			}

			return calendar;


		}
	}
}

