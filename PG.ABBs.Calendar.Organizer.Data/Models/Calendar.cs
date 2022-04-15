using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Data.Models
{
	public class Calendar
	{
		[Key]
		public Guid CalenderId { get; set; }
		public string UuidHash { get; set; }
		public DateTime DueDate { get; set; }
		public string DueDateHash { get; set; }
		public DateTime DateCreated { get; set; }
		public string Locale { get; set; }


	}
}
