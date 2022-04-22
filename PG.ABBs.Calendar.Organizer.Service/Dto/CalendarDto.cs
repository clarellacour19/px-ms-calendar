using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Service.Dto
{
	public class CalendarDto
	{
		public Guid CalendarId { get; set; }
		public string UuidHash { get; set; }
		public DateTime DueDate { get; set; }
		public string DueDateHash { get; set; }
		public DateTime DateCreated { get; set; }
		public string Locale { get; set; }
		public string CdnUrl { get; set; }
	}
}