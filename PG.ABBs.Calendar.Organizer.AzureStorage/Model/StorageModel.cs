using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.AzureStorage
{
	public class StorageModel
	{
		public const string SectionName = "AzureStorage";

		public string ContainerName { get; set; }
		public string ConnectionString { get; set; }
	}
}
