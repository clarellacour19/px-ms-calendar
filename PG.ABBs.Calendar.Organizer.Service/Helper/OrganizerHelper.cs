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
	    

        // Check if "hreflang" from CMS and database is same
        public static bool IsSame(JObject fields, JObject hreflangTagBindings)
        {
            var hreflang = new JObject(fields.Value<JObject>("hreflang"));
            return JToken.DeepEquals(hreflangTagBindings, hreflang);

        }


        // Replace existed "hreflang" field with "hreflang" on database
        public static JObject Merge(JObject fields, JObject hreflangTagBindings)
        {
            fields.Remove("hreflang");
            fields.Add("hreflang", hreflangTagBindings);

            return fields;
        }

        // Format "hreflang" from database model to JObject
        public static JObject FormatToHrefLangElement(IList<HreflangTagBinding> hreflangTagBindings,  string market)
        {
            JArray array = new JArray();
            if (hreflangTagBindings.Any())
            {
                foreach (var hreflangTagBinding in hreflangTagBindings)
                {
                    var element = new JObject
                    {
                        {"url", hreflangTagBinding.Url},
                        {"locale", hreflangTagBinding.Language}
                    };
                    array.Add(element);
                }
            }

            return new JObject { { market, array } };

        }

        public static T FormatEntryField<T> ( string fields) where T: class
        {
            return JsonConvert.DeserializeObject<T>(fields);
        }

        public static string GetCorrespondingGpocId(string fields,string key)
        {
	        var deserializeObject = JsonConvert.DeserializeObject<HrefLangContainer>(fields);
	        var dictionary = deserializeObject.GpocId;

	        if (dictionary == null) return null;
            string result;
            dictionary.TryGetValue(key, out result);

            return result;

        }

        public static string GetCorrespondingPath(string fields, string key)
        {
	        var deserializeObject = JsonConvert.DeserializeObject<HrefLangContainer>(fields);
	        var dictionary = deserializeObject.Path;

	        if (dictionary == null) return null;
	        string result;
	        dictionary.TryGetValue(key, out result);

	        return result;
        }


        public static T GetCorrespondingEntryValue<T>(
            Dictionary<string, T> dictionary,
            string key) where T : class
        {
            if (dictionary == null) return null;
            T result;
            dictionary.TryGetValue(key, out result);

            return result;
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
