using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PG.ABBs.Calendar.Organizer.Data.Models
{
	public class UserCalendar
	{
		[Key] public Guid UserCalendarId { get; set; }
		public string Uuidhash { get; set; }
		public string DueDateHash { get; set; }
		public string CalendarId { get; set; }
		public string locale { get; set; }
	}

	[Keyless]
	public class GetCountInTable
	{
		public int Count { get; set; }
	}
}