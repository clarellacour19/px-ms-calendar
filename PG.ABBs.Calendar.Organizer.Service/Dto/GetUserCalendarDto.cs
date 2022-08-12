using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Service.Dto
{
	public class GetUserCalendarDto
	{
		
		public string locale { get; set; }
		public string uuidHash { get; set; }
		public int? limit { get; set; }
		public string sorting { get; set; }
		public string? AccessToken { get; set; }
		public string? UserId { get; set; }
	}
}