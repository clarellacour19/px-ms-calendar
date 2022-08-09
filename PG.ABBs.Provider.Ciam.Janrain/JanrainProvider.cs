using Newtonsoft.Json;
using PG.ABBs.CalendarOrganizer.Core.DTO;
using PG.ABBs.Provider.Ciam.CiamProvider;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PG.ABBs.Provider.Ciam.Janrain
{
    public class JanrainProvider : ICiamProvider
    {
        public string Name { get; set; } = "Janrain";

        public CiamBase FetchProfile(NameValueCollection collection, Dictionary<string, string> content)
        {
            var apiUrl = collection["Url"];
            var response = PostData(apiUrl, content);
            if (!response.Success)
            {
                return null;
            }
            var data = JsonObject.Parse(response.Data.ToString())["result"]?.ToString() ?? String.Empty;
            var profile = JsonConvert.DeserializeObject<CiamBase>(data) ?? new CiamBase();
            return profile;


        }

        private Result PostData(string apiUrl, Dictionary<string, string> content)
        {
            var client = new HttpClient();

            try
            {
                var request = client.PostAsync(apiUrl, new FormUrlEncodedContent(content));

                if (!request.Result.IsSuccessStatusCode)
                {
                    return new Result()
                    {
                        Success = false,
                        Message = "Connection Error to JANRAIN"
                    };
                }

                using var reader = new StreamReader(request.Result.Content.ReadAsStream());
                var response = reader.ReadToEnd();


                if (string.IsNullOrEmpty(response))
                    return new Result() { Success = false, Message = "Connection Error to JANRAIN" };


                return new Result()
                {
                    Success = true,
                    Data = response
                };

            }
            catch
            {
                return new Result()
                {
                    Success = false,
                    Message = "Connection Error to JANRAIN"
                };
            }
        }
    }

    


}
