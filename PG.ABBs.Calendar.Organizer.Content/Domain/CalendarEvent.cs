using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
	public class CalendarEvent
	{
		public SystemProperties Sys { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Period { get; set; }
		public int Number { get; set; }
		public string Url { get; set; }
		public string Type { get; set; }
		public string Locale { get; set; }

	}
}
