using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Data.Models
{
	public class Events
	{
		[Key] public Guid EventId { get; set; }

		public string ContentId { get; set; }
		public DateTime LastCreated { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Period { get; set; }
		public int Number { get; set; }
		public string URL { get; set; }
		public string Type { get; set; }
		public string Locale { get; set; }
	}
}