using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Service.Dto
{
	public class GenerateCalendarDto
	{	
		public string site { get; set; }
		public string locale { get; set; }
		public string uuidHash { get; set; }
		public string dueDate { get; set; }
	}
}
