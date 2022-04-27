using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
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

		public async Task UploadCalendarsAsync(string market, List<Ical.Net.Calendar> listOCalendars)
		{
			try
			{
				int count = 0;

				BlobUploadOptions options = new BlobUploadOptions
				{
					TransferOptions = new StorageTransferOptions
					{
						// Set the maximum number of workers that 
						// may be used in a parallel transfer.
						MaximumConcurrency = 8,

						// Set the maximum length of a transfer to 50MB.
						MaximumTransferSize = 50 * 1024 * 1024
					}
				};

				// Create a queue of tasks that will each upload one file.
				var tasks = new Queue<Task<Response<BlobContentInfo>>>();


				// Iterate through the files
				foreach (Ical.Net.Calendar calendar in listOCalendars)
				{
					var iCalSerializer = new CalendarSerializer();
					string serializedCalendar = iCalSerializer.SerializeToString(calendar);
					var bytesCalendar = Encoding.ASCII.GetBytes(serializedCalendar);

					BlobClient blob = container.GetBlobClient(Path.Combine($"{market}/{calendar.Name}.ics"));


					using (MemoryStream memoryStream = new MemoryStream(bytesCalendar))
					{
						// Add the upload task to the queue
						tasks.Enqueue(blob.UploadAsync(memoryStream, options));
					}
				}

				// Run all the tasks asynchronously.
				await Task.WhenAll(tasks);
			}
			catch (RequestFailedException ex)
			{
				Console.WriteLine($"Azure request failed: {ex.Message}");
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine($"Error parsing files in the directory: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");
			}
		}

		public void UploadCalendar(Ical.Net.Calendar CalendarName, string market, string duedateHash)
		{
			try
			{
				var iCalSerializer = new CalendarSerializer();
				string serializedCalendar = iCalSerializer.SerializeToString(CalendarName);
				var bytesCalendar = Encoding.ASCII.GetBytes(serializedCalendar);

				var blockBlob = container.GetBlobClient(Path.Combine($"{market}/{duedateHash}.ics"));

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

		public async Task DeleteCalendarAsync(string market, List<string> listOfCals)
		{
			try
			{
				var tasks = new Queue<Task<Response>>();
				foreach (var calenderName in listOfCals)
				{
					var blob = container.GetBlobClient($"{market}/{calenderName}.ics");
					if (blob.ExistsAsync().Result)
					{
						tasks.Enqueue(blob.DeleteAsync());
					}
				}

				await Task.WhenAll(tasks);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public void DeleteCalendar(string CalendarName, string market)
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