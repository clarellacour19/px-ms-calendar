using System;
using System.Collections.Generic;
using System.Text;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace PG.ABBs.Calendar.Organizer.Content.Domain
{
	public class Item
	{
		
		public string path { get; set; }

		
		public string gpocId { get; set; }

		
		public SystemProperties sys { get; set; }
	}
}
