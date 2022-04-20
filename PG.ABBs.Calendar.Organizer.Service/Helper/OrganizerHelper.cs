// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HreflangtagHelper.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Inteface to the content repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PG.ABBs.Calendar.Organizer.Content.Configuration;
using PG.ABBs.Calendar.Organizer.Content.Domain;
using PG.ABBs.Calendar.Organizer.Content.GraphQL;

using PG.ABBs.Calendar.Organizer.Content.Domain;

namespace PG.ABBs.Calendar.Organizer.Service.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PG.ABBs.Calendar.Organizer.Data.Models;

    public static class OrganizerHelper
    {
        public static Collection entryList = new Collection
        {
            items = new List<Item>()
        };


        public static Dictionary<string, object> StoredProcArgument(Dictionary<string, object> dict)
        {
	        return dict;
        }

        public static string CreateMD5(string input)
        {
	        // Use input string to calculate MD5 hash
	        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
	        {
		        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
		        byte[] hashBytes = md5.ComputeHash(inputBytes);

		        return Convert.ToHexString(hashBytes); // .NET 5 +

		        // Convert the byte array to hexadecimal string prior to .NET 5
		        // StringBuilder sb = new System.Text.StringBuilder();
		        // for (int i = 0; i < hashBytes.Length; i++)
		        // {
		        //     sb.Append(hashBytes[i].ToString("X2"));
		        // }
		        // return sb.ToString();
	        }
        }

        public static async Task<Item> GetEntryAsync(MarketSettings marketSettings,string entryType, string entryId)
        {

	        var apiURI = new StringBuilder();
	        apiURI.Append($"https://graphql.contentful.com/content/v1/");
	        apiURI.Append($"spaces/{marketSettings.SpaceId}/environments/{marketSettings.Environment}");

	        var entry = new Item
	        {
		        path = null,
		        gpocId = null
	        };
	        try
	        {
		        if (entryList.items.Where(e => e.sys.Id.Contains(entryId)).Any())
		        {
			        entry = entryList.items.FirstOrDefault(e => e.sys.Id.Equals(entryId));

			        return entry;
		        }
		        else
		        {

			        entryList.items.Clear();

                    entryList = await GraphQLConsumer.GetAllEntries(apiURI.ToString(), marketSettings.DeliveryApiKey,
				        entryType);


			        entry = entryList.items.FirstOrDefault(e => e.sys.Id.Equals(entryId));



			        return entry;

		        }
            }
	        catch (System.Exception e)
	        {
		        Console.WriteLine(e);
		        return null;
	        }

        }

    }
}
