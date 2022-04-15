// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRepository.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the implementation on the IContentRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using PG.ABBs.Calendar.Organizer.Content.RetryPolicy;

namespace PG.ABBs.Calendar.Organizer.Content.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Contentful.Core.Models;
    using Contentful.Core.Search;
    using Microsoft.Extensions.Logging;


    public class DefaultRepository : IContentRepository
    {
        public string ProviderName { get; } = "Default";

        private readonly ILogger logger;
        private  readonly IRetryPolicy _retryPolicy;

        public ClientFactory Factory { get; set; }

        public DefaultRepository(ClientFactory factory, ILogger<DefaultRepository> loggerProvider,IRetryPolicy retry)
        {
            this.Factory = factory;
            this.logger = loggerProvider;
            this._retryPolicy = retry;
        }

        public async Task<IEnumerable<T>> GetAll<T>(string locale, string contentType) where T : class
        {
            var client = this.Factory.GetClient(locale);
            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }
            var result = new List<T>();
            int batchNumber = 0;
            int limit = 100;
            //Always query till you get all Contentfull item
            var errorMessage = "Exception in GetAll method ";
            await _retryPolicy.RetryPolicyAsync(errorMessage).ExecuteAsync(async () =>
            {
                while (true)
                {
                    var builder = new QueryBuilder<T>().Skip(batchNumber * limit).Limit(limit).LocaleIs(locale).ContentTypeIs(contentType);
                    var queryResult = await client.GetEntries(builder);
                    result.AddRange(queryResult.Items);
                    batchNumber++;
                    if (queryResult.Total <= (batchNumber * limit))
                    {
                        break;
                    }
                }
            });

            return result;
        }

        public async Task<IEnumerable<T>> GetAllShared<T>(string locale, string contentType, string sharedLocale, string defaultLocale) where T : class
        {
            try
            {
                return await GetShared<T>(locale, contentType, sharedLocale);
            }
            catch (Exception e)
            {
                this.logger.LogWarning($"No Contentful SharedSettings found for locale {locale}. Using to {defaultLocale}");
                return await GetShared<T>(defaultLocale, contentType, sharedLocale);
            }
        }

        public async Task<IEnumerable<T>> GetAllGpoc<T>(string locale, string contentType) where T : class
        {
            var client = this.Factory.GetClient(locale);
            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }
            var result = new List<T>();
            int batchNumber = 0;
            int limit = 100;
            //Always query till you get all Contentful item
            var errorMessage = "Exception in GetAllGpoc method ";
            await _retryPolicy.RetryPolicyAsync(errorMessage).ExecuteAsync(async () =>
            {
                while (true)
                {
                    //TODO : Find a way to get all Content containing GPOCID
                    var builder = new QueryBuilder<T>().Skip(batchNumber * limit).Limit(limit).LocaleIs(locale).ContentTypeIs(contentType).FieldExists("fields.gpocId");
                    var queryResult = await client.GetEntries(builder);
                    result.AddRange(queryResult.Items);
                    batchNumber++;
                    if (queryResult.Total <= (batchNumber * limit))
                    {
                        break;
                    }
                }
            });

            return result;
        }

        public async Task<IEnumerable<Entry<dynamic>>> GetAllGpocEntry(string locale, string contentType)
        {
            var client = this.Factory.GetManagementClient(locale);
            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }
            var result = new List<Entry<dynamic>>();
            int batchNumber = 0;
            int limit = 100;

            var errorMessage = "Exception in GetAllGpocEntry method ";
            await _retryPolicy.RetryPolicyAsync(errorMessage).ExecuteAsync(async () =>
            {
                while (true)
                {
                    var builder = new QueryBuilder<Entry<dynamic>>().Skip(batchNumber * limit).Limit(limit).LocaleIs(locale).ContentTypeIs(contentType).FieldExists("fields.gpocId");

                    var queryResult = await client.GetEntriesCollection(builder);

                    result.AddRange(queryResult.Items);
                    batchNumber++;
                    if (queryResult.Total <= (batchNumber * limit))
                    {
                        break;
                    }
                }
            });
            
            return result;
        }

        public async Task<IEnumerable<T>> GetRelatedEntries<T>(string entryId, string locale, bool specifyType = true) where T : class
        {
            var client = this.Factory.GetClient(locale);

            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }

            var builder = new QueryBuilder<T>().LocaleIs(locale).LinksToEntry(entryId);
            var result = await client.GetEntries(builder.Include(3));

            return result.Items;
        }

        public async Task<T> Single<T>(string entryId, string locale) where T : class
        {
            var client = this.Factory.GetClient(locale);

            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }

            var result = await client.GetEntry<T>(entryId);

            return result;
        }

        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, object>> @where, string value, string locale) where T : class
        {
            var client = this.Factory.GetClient(locale);

            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }

            var builder = new QueryBuilder<T>().LocaleIs(locale).FieldEquals(@where, value).Include(4);
            var result = await client.GetEntries(builder);

            return result.Items;
        }

        public async Task<Entry<dynamic>> Update(Entry<dynamic> entry, string locale)
        {
            var client = this.Factory.GetManagementClient(locale);
            if (client == null)
            {
                throw new CultureNotFoundException($"No Contentful Settings found for locale {locale}");
            }
            var modifiedEntry = await client.CreateOrUpdateEntry(entry, version: entry.SystemProperties.Version);

            if (entry.SystemProperties.Version == (entry.SystemProperties.PublishedVersion + 1) && modifiedEntry?.SystemProperties?.Version != null)
            {
                var version = modifiedEntry.SystemProperties.Version ?? 0;
                await client.PublishEntry(modifiedEntry.SystemProperties.Id, version);
            }

            return modifiedEntry;
        }

        public async Task<IEnumerable<T>> GetShared<T>(string locale, string contentType, string sharedLocale) where T : class
        {
	        var client = this.Factory.GetSharedClient(sharedLocale);
	        if (client == null)
	        {
		        throw new CultureNotFoundException($"No Contentful Settings found for locale {sharedLocale}");
	        }
	        var result = new List<T>();
	        int batchNumber = 0;
	        int limit = 100;

            //Always query till you get all Contentfull item
            var errorMessage = "Exception in GetShared method ";
            await _retryPolicy.RetryPolicyAsync(errorMessage).ExecuteAsync(async () =>
            {
                while (true)
                {
                    var builder = new QueryBuilder<T>().Skip(batchNumber * limit).Limit(limit).LocaleIs(locale).ContentTypeIs(contentType);
                    var queryResult = await client.GetEntries(builder);
                    result.AddRange(queryResult.Items);
                    batchNumber++;
                    if (queryResult.Total <= (batchNumber * limit))
                    {
                        break;
                    }
                }
            });

            return result;
        }
    }
}