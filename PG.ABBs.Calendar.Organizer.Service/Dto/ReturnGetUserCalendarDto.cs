using PG.ABBs.Calendar.Organizer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Service.Dto
{
	public class ReturnGetUserCalendarDto
	{
		public List<CalendarDto> Calendar { get; set; }
	}
}