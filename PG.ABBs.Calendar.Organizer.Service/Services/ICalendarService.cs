using PG.ABBs.Calendar.Organizer.Service.Dto;
using System;

namespace PG.ABBs.Calendar.Organizer.Service.Services
{
	public interface ICalendarService
	{
		Task<List<string>> BatchUpdateCalendar(BatchUpdateCalendarDto Dto);

		ReturnGetUserCalendarDto GetUserCalendar(GetUserCalendarDto Dto);

		CalendarDto GenerateCalendar(GenerateCalendarDto Dto);

		Task<byte[]> DownloadCalendar(string path);

	}
}