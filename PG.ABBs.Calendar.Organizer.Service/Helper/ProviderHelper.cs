using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PG.ABBs.Calendar.Organizer.Service.Helper
{
    public class JanrainProvider
    {
        public string Locale { get; set; }
        public string Url { get; set; }
    }

    public static class ProviderConstant
    {
        public const string AccessToken = "access_token";
        public const string ConsumerId = "consumerID";
        public const string uuid = "uuid";
        public const string id = "id";
        public const string JanrainProviders = "JanrainProviders";
        public const string JanrainDefaultUrl = "https://procter-gamble.eu-dev.janraincapture.com/entity";
    }
    public static class ProviderHelper
    {
        public static async Task<bool> VerifyProfile(List<JanrainProvider> janrainProviders,
            string encryptionV2Key,
            string ivvar,
            string userId,
            string accessToken,
            string locale)
        {

            //create request
            var apiUrl = janrainProviders.FirstOrDefault(o => o.Locale == locale)?.Url ?? ProviderConstant.JanrainDefaultUrl;
            var client = new HttpClient();
            var data = new JsonObject();

            accessToken = DecryptRequestV2(accessToken, encryptionV2Key, ivvar);

            var content = new Dictionary<string, string>();
            content.Add(ProviderConstant.AccessToken, accessToken);


            var request = await client.PostAsync(apiUrl, new FormUrlEncodedContent(content));

            if (!request.IsSuccessStatusCode)
            {
                return false;
            }

            var result = JObject.Parse(await request.Content.ReadAsStringAsync());
            var consumerId = result["result"]?[ProviderConstant.ConsumerId]?.ToString();
            var uuid = result["result"]?[ProviderConstant.uuid]?.ToString();
            var id = result["result"]?[ProviderConstant.id]?.ToString();

            if (userId == consumerId || userId == uuid || userId == id)
            {
                return true;
            }
            return false;

        }

        public static string DecryptRequestV2(string accessToken, string keyVar, string IVVar)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {

                var encrytpedBytes = Convert.FromBase64String(accessToken);
                byte[] IV = Encoding.UTF8.GetBytes(IVVar);
                byte[] key = Encoding.UTF8.GetBytes(keyVar);

                string decrypted;
                using (var alg = SymmetricAlgorithm.Create("Aes"))
                {

                    using (var transform = alg.CreateDecryptor(key, IV))
                    using (var msIn = new MemoryStream(encrytpedBytes))
                    using (var msOut = new MemoryStream())
                    using (var crypto = new CryptoStream(msIn, transform, CryptoStreamMode.Read))
                    {
                        crypto.CopyTo(msOut);
                        crypto.Close();
                        var aa = msOut.ToArray();
                        decrypted = System.Text.Encoding.UTF8.GetString(aa);
                    }

                    alg.Clear();

                    return decrypted;
                }

            }
            return null;
        }
    }
}
