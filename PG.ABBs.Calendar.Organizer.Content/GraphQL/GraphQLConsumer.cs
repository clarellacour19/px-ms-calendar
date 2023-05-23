using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using GraphQL;
using GraphQL.Client;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PG.ABBs.Calendar.Organizer.Content.Domain;

namespace PG.ABBs.Calendar.Organizer.Content.GraphQL
{	

	
	public static class GraphQLConsumer
	{

		private static GraphQLHttpClient graphQLHttpClient;


		public static async Task<dynamic> GetAllEntries(string apiURI, string accessToken, string collection )
		{


			var uri = new Uri(apiURI);
			var graphQLOptions = new GraphQLHttpClientOptions
			{
				EndPoint = uri,
				HttpMessageHandler = new NativeMessageHandler(),
				
			};

			graphQLHttpClient = new GraphQLHttpClient(graphQLOptions, new NewtonsoftJsonSerializer());

			graphQLHttpClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer  {accessToken}");

			var query = new GraphQLRequest
			{
				Query = $@"
               {{
				  {collection}Collection(where: {{AND: [{{gpocId_exists: true}}]}}, limit: 10000) {{
					items {{
					  gpocId
					  path
					  sys{{
						id
					  }}
					}}
				  }}
				}}
				"
			};

			var response = await graphQLHttpClient.SendQueryAsync<JObject>(query);
			var entryCollection = response.Data[$"{collection}Collection"];

			var result = entryCollection.ToObject<Collection>();

			return result;

		}
	}
}
