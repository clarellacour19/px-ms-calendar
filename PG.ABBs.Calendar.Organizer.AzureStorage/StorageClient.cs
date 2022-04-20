using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ical.Net;
using Ical.Net.Serialization;
using Microsoft.Extensions.Options;

namespace PG.ABBs.Calendar.Organizer.AzureStorage
{
	public class StorageClient
	{
		private readonly IOptions<StorageModel> azureStorage;
		private BlobContainerClient container;

		public StorageClient(IOptions<StorageModel> azureStorage)
		{
			this.azureStorage = azureStorage;

			if (this.azureStorage.Value.ConnectionString != null && this.azureStorage.Value.ContainerName != null)
			{
				container = new BlobContainerClient(this.azureStorage.Value.ConnectionString,
					this.azureStorage.Value.ContainerName);
				container.CreateIfNotExists(PublicAccessType.Blob);
			}

		}

		public void UploadCalendar(Ical.Net.Calendar CalendarName,string market)
		{
			try
			{
				var iCalSerializer = new CalendarSerializer();
				string serializedCalendar = iCalSerializer.SerializeToString(CalendarName);
				var bytesCalendar = Encoding.ASCII.GetBytes(serializedCalendar);

				var blockBlob = container.GetBlobClient(Path.Combine($"{market}/{CalendarName.Name}.ics"));

				using (MemoryStream memoryStream = new MemoryStream(bytesCalendar))
				{
					blockBlob.UploadAsync(memoryStream);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			

		}

		public void DeleteCalendar(string CalendarName,string market)
		{
			try
			{
				if (!CalendarName.Equals(null))
				{
					var blob = container.GetBlobClient($"{market}/{CalendarName}.ics");
					if (blob.ExistsAsync().Result)
					{
						blob.DeleteAsync();
						
					}
				}

				
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;


			}
		}
	}
}
