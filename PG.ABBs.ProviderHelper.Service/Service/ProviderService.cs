using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PG.ABBs.Provider.Ciam.CiamProvider;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PG.ABBs.ProviderHelper.Service
{

    public class CiamMarket
    {
        public string Locale { get; set; }
        public string ProviderName { get; set; }
    }

    public static class ProviderConstant
    {
        public const string AccessToken = "access_token";
        public const string ConsumerId = "consumerID";
        public const string uuid = "uuid";
        public const string id = "id";
        public const string CiamMarkets = "CiamMarkets";
        public const string CiamProviders = "CiamProviders";
        public const string JanrainDefaultUrl = "https://procter-gamble.eu-dev.janraincapture.com/entity";
    }
    public class ProviderService : IProviderService
    {
        private readonly CiamProviderManager _manager;
        private readonly ILogger _logger;

        public ProviderService(
           CiamProviderManager manager,
           ILogger<ProviderService> loggerProvider)
        {
            this._logger = loggerProvider;
            this._manager = manager;
        }

        public bool VerifyProfile(
            string encryptionV2Key,
            string ivvar,
            string userId,
            string accessToken,
            string locale)
        {
            try
            {
                //create request

                accessToken = DecryptRequestV2(accessToken, encryptionV2Key, ivvar);

                var content = new Dictionary<string, string>();
                content.Add(ProviderConstant.AccessToken, accessToken);

                var provider = this._manager.GetMarketProvider(locale);
                var settings = this._manager.GetProviderSettings(locale);
                var collection = new NameValueCollection();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(settings))
                {
                  string value = propertyDescriptor.GetValue(settings).ToString();
                  collection.Add(propertyDescriptor.Name, value);
                }
                var profile = provider.FetchProfile(collection, content);

                if (userId == profile.ConsumerId || userId == profile.Uuid)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error during profile authentication: {ex.Message} - {DateTime.UtcNow} - {ex.StackTrace}");
                throw new Exception($"Error during profile authentication: {ex.Message} - {DateTime.UtcNow}");
            }
            

        }

        private string DecryptRequestV2(string accessToken, string keyVar, string IVVar)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
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
                catch(Exception ex)
                {
                    this._logger.LogError($"Error during key decrypting: {ex.Message} - {DateTime.UtcNow} - {ex.StackTrace}");
                    throw new Exception(message: $"access token decrypting error");
                }
                

            }
            return null;
        }

       
    }
}
