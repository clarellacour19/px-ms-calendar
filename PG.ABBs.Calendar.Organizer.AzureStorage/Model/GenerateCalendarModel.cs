using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.AzureStorage.Model
{
	public class GenerateCalendarModel
	{	
		public Ical.Net.Calendar Calendar { get; set; }
		public string DueDateHash { get; set; }
	}
}
