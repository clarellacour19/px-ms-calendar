using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PG.ABBs.Calendar.Organizer.Service.Dto;

namespace PG.ABBs.Calendar.Organizer.Service.Services
{
	public interface ICalendarService
	{
		Task<Object> BatchUpdateCalendar(BatchUpdateCalendarDto Dto);

		Task<List<Data.Models.Calendar>> GetUserCalendar(GetUserCalendarDto Dto);

		Object GenerateCalendar(GenerateCalendarDto Dto);
	}
}
