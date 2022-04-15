// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HrefLangService.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Regroup all the service needed by hreflang tag
// </summary>
// -------------------------------------------------------------------------------------------------------------------- 


using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Contentful.Core.Errors;
using Newtonsoft.Json;

namespace PG.ABBs.Calendar.Organizer.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contentful.Core.Models;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using PG.ABBs.Calendar.Organizer.Content;
    using PG.ABBs.Calendar.Organizer.Content.Configuration;
    using PG.ABBs.Calendar.Organizer.Content.Domain;
    using PG.ABBs.Calendar.Organizer.Content.Repository;
    using PG.ABBs.Calendar.Organizer.Data.Constant;
    using PG.ABBs.Calendar.Organizer.Data.Context;
    using PG.ABBs.Calendar.Organizer.Data.Models;
    using PG.ABBs.Calendar.Organizer.Data.Repositories;
    using PG.ABBs.Calendar.Organizer.Service.Helper;
    public class HrefLangService : IHrefLangService
    {

        #region Fields

        private readonly IUnitOfWork<DataContext> unitOfWork;

        private readonly IOptions<List<MarketSettings>> marketSettings;

        private readonly IOptions<SharedMarketSettings> sharedMarketSettings;

        private readonly IOptions<List<string>> contentTypeSettings;

        private readonly ContentManager contentManager;

        private readonly MarketSettingsHelper marketSettingsHelper;

        private readonly ILogger logger;

        #endregion       

        #region Constructors and Destructors

        public HrefLangService(
            IUnitOfWork<DataContext> unitOfWork, 
            ContentManager contentManager, 
            IOptions<List<MarketSettings>> marketSettings,
            IOptions<SharedMarketSettings> sharedMarketSettings,
            IOptions<List<string>> contentTypeSettings,
            MarketSettingsHelper marketSettingsHelper,
            ILogger<HrefLangService> loggerProvider)
        {
            this.unitOfWork = unitOfWork;
            this.contentManager = contentManager;
            this.marketSettings = marketSettings;
            this.sharedMarketSettings = sharedMarketSettings;
            this.contentTypeSettings = contentTypeSettings;
            this.marketSettingsHelper = marketSettingsHelper;
            this.logger = loggerProvider;
        }

		

		#endregion

		#region Methods

		/// <summary>
		///     Job Execution.
		///     See Feature > HREF Lang JOB section in the Tech Approach doc.
		/// </summary>
		/// <returns></returns>
		public async Task<Object> Process()
        {
            var beginTime = DateTime.UtcNow;
            DateTime endTime;
            this.logger.LogWarning($"Href lang batch begin : {beginTime}");
            var errorList = new List<String>();
           
            try
            {
                var repository = this.contentManager.GetRepository();
                foreach (var errors in this.marketSettings.Value.Select(market => ProcessBatch(market, repository).Result).Where(errors => errors.Any()))
                {
                    errorList.AddRange(errors);
                }
            }
            catch (Exception ex)
            {
                errorList.Add($"Error during process: {ex.Message} - {DateTime.UtcNow}");
                this.logger.LogError($"Error during process: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
            }
            finally
            {
                endTime = DateTime.UtcNow;
                this.logger.LogWarning($"Href lang batch End : {endTime}");
            }
            var time = endTime.Subtract(beginTime);

            var result = new
            {
                BeginTime = beginTime,
                EndTime = endTime,
                ProcessTimeTook = $"{time.Hours}h:{time.Minutes}mm:{time.Seconds}s:{time.Milliseconds}ms",
                Errors = errorList
            };

            return result;
        }

        public async Task<Entry<dynamic>> ProcessItem( Entry<dynamic> content, MarketSettings market, IContentRepository repository, string domainUrl)
        {

	        try
            {
	            var sysId = content.SystemProperties.Id;
	            var entryAPI = await OrganizerHelper.GetEntryAsync(market, content.SystemProperties.ContentType.SystemProperties.Id, sysId);
	            string path;
	            string gpocId;


	            if (entryAPI?.gpocId == null && entryAPI?.path==null)
	            {
		            HrefLangContainer formatedFields = OrganizerHelper.FormatEntryField<HrefLangContainer>(content.Fields.ToString());
		            path = OrganizerHelper.GetCorrespondingEntryValue(formatedFields.Path, market.Language);
		            gpocId = OrganizerHelper.GetCorrespondingEntryValue(formatedFields.GpocId, market.Language);
                    
                }
                else
                {
	                 path = entryAPI.path ?? string.Empty;
	                 gpocId = entryAPI.gpocId ?? string.Empty;
                }

	            if (gpocId.Equals("NA"))
	            {
		            gpocId = string.Empty;
	            }

	            var contentUrl = domainUrl + (path ?? "");

                if (!string.IsNullOrEmpty(gpocId))
                {
                var argsToGet = new Dictionary<string, object>
                {
                    { "local", market.Language },
                    { "itemId", content.SystemProperties.Id },
                    { "AlternateUrlID", gpocId },
                    { "url" , contentUrl },
                    {"siteName", market.SiteName},
                    {"siteId", market.SiteId}
                };
                var listHreflangTag = this.unitOfWork.GetRepository<HreflangTagBinding>().ExecuteStoredProcedure(Constant.DatabaseObject.StoredProcedure.GetRestageHreflangTagBinding, argsToGet);
                var hreflang = OrganizerHelper.FormatToHrefLangElement(listHreflangTag, market.Language);

                if (hreflang.TryGetValue(market.Language, out var value) && !value.Any())
                {
	                hreflang[market.Language] = new JArray();
                }


                if (JsonConvert.SerializeObject(hreflang[market.Language])
	                .ToString().Equals("[]")&& JsonConvert.SerializeObject(content.Fields[market.Language]).ToString().Equals("[]"))
                {
	                return content;
	            }


                JObject initialContentField =  new JObject(content.Fields);
                content.Fields = OrganizerHelper.Merge(content.Fields, hreflang);
                JObject finalContentFields = content.Fields;
                
                this.logger.LogWarning($"Process Item on market {market.Language}, " +
                                       $"contentType {content.SystemProperties.ContentType.SystemProperties.Id}, " +
                                       $"id {content.SystemProperties.Id} ");

                    if (finalContentFields.ContainsKey("hreflang") &&
                    !JToken.DeepEquals(initialContentField, finalContentFields) &&
                    content.SystemProperties.ArchivedAt == null && content.SystemProperties.ArchivedBy == null &&
                    content.SystemProperties.ArchivedVersion == null)
                {
                    var message = $"Updating content on market {market.Language}, " +
                                  $"contentType {content.SystemProperties.ContentType.SystemProperties.Id}, " +
                                  $"id {content.SystemProperties.Id} ";
                    this.logger.LogWarning(message);
                    await repository.Update(content, market.Language);
                }


			}

            return content;
            }
            catch (Exception e)
            {
	            Console.WriteLine(e);
	            throw;
	            
            }
        }

        public void DeleteEntry(DeletedEntity entry)
        {
	        try
	        {
		        var spaceId = entry.spaceId;
		        var locale = marketSettingsHelper.MarketSettingsBySpaceId[spaceId].Language;
		        var entryId = entry.entityId;

		        var argsToGet = new Dictionary<string, object>
		        {
			        { "local", locale },
			        { "itemId", entryId }
		        };

		        this.unitOfWork.GetRepository<HreflangTagBinding>().ExecuteNonQueryStoredProcedure(Constant.DatabaseObject.StoredProcedure.DeleteEntryHreflangTagBinding, argsToGet);
		        this.logger.LogInformation($"Href lang batch delete process success -- entry id : {entryId}");
                
            }
	        catch (Exception ex)
	        {
		        this.logger.LogError($"Error during delete process --entry id: {entry.entityId}: {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
                
	        }
	        
        }

        public List<Entry<dynamic>> FilterDuplicates(List<Entry<dynamic>> gpocContentList, MarketSettings market)
        {

	        var orderedGpocContentList = gpocContentList.OrderBy(o =>
		        OrganizerHelper.GetCorrespondingGpocId(o.Fields.ToString(), market.Language)).ToList();

	        Entry<dynamic> previousEntry = null;
	        Entry<dynamic> entry = null;
            var logString = new StringBuilder();

            for (int i = 0; i < orderedGpocContentList.Count; i++)
            {
	            entry = orderedGpocContentList[i];

	            var entryGpocId = OrganizerHelper.GetCorrespondingGpocId(entry.Fields.ToString(), market.Language);
	            var previousEntryGpocId = (previousEntry != null) ? OrganizerHelper.GetCorrespondingGpocId(previousEntry.Fields.ToString(), market.Language) : "";

	            if (entryGpocId.ToString().Equals(previousEntryGpocId.ToString()))
	            {
		            if (!string.IsNullOrEmpty(entryGpocId))
		            {
			            var entryPath =
				            OrganizerHelper.GetCorrespondingPath(entry.Fields.ToString(), market.Language);
                        var previousEntryPath = OrganizerHelper.GetCorrespondingPath(previousEntry?.Fields.ToString(), market.Language);

                        if (!string.IsNullOrEmpty(entryPath) && !string.IsNullOrEmpty(previousEntryPath))
                        {
	                        logString.AppendLine(
		                        $"Market: {market.Language} - Entry Id: '{entry.SystemProperties.Id}' & Path: {entryPath} has duplicate GpocId '{entryGpocId}' was NOT processed -- Previous Entry ID with same GpocId: '{previousEntry?.SystemProperties.Id} & Path : {previousEntryPath}'");
                        }
                        else
                        {
	                        logString.AppendLine(
		                        $"Market: {market.Language} - Entry Id: '{entry.SystemProperties.Id}' has duplicate GpocId '{entryGpocId}' was NOT processed -- Previous Entry ID with same GpocId: '{previousEntry?.SystemProperties.Id}'");
                        }


                    }

		            orderedGpocContentList.Remove(entry);
		            i--;
                }
	            else
	            {
		            previousEntry = entry;
	            }

            }

            this.logger.LogWarning($"{logString}");
            return orderedGpocContentList;
        }

        public async Task<object> ProcessByMarket(string sitename)
        {
            var beginTime = DateTime.UtcNow;
            DateTime endTime;
            this.logger.LogWarning($"Href lang batch begin By Market : {beginTime}");

            var errorList = new List<string>();

            try
            {
                var repository = this.contentManager.GetRepository();
                var market = this.marketSettings.Value.FirstOrDefault(x => x.SiteName.Equals(sitename));

                if (market != null)
                {
                    this.logger.LogWarning($"Href lang executed for : market {market.Language} - {market.SpaceId} (By Market)");
                    var errors = ProcessBatch(market, repository).Result;
                    if (errors.Any())
                    {
                        errorList.AddRange(errors);
                    }
                }
                else
                {
                    this.logger.LogWarning($"Market {sitename} does not exists");
                }
            }
            catch (Exception ex)
            {
                errorList.Add($"Error during process: {ex.Message} - {DateTime.UtcNow}");
                this.logger.LogError($"Error during process: market {sitename} {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace} (By Market)");
            }
            finally
            {
                endTime = DateTime.UtcNow;
                this.logger.LogWarning($"Href lang batch End : {endTime} By Market {sitename}");
            }

            var time = endTime.Subtract(beginTime);

            var result = new
            {
                BeginTime = beginTime,
                EndTime = endTime,
                ProcessTimeTook = $"{time.Hours}h:{time.Minutes}mm:{time.Seconds}s:{time.Milliseconds}ms",
                Errors = errorList
            };

            return result;
        }

        protected async Task<List<string>> ProcessBatch(MarketSettings market, IContentRepository repository)
        {
            var errorList = new List<string>();
            try
            {
                var siteSettings =
                    (await repository.GetAll<SiteSettingsContent>(market.Language, ContentSettings.SiteSetting))
                    .ToList();
                var domainUrl = siteSettings.Any()
                    ? siteSettings[0].Domain
                    : string.Empty;
                var sharedSettings =
                    (await repository.GetAllShared<SharedSetting>(market.Language, ContentSettings.SiteSetting, sharedMarketSettings.Value.Language, sharedMarketSettings.Value.DefaultLocale))
                    .ToList();
                var listOfContentType = sharedSettings.Any()
                    ? sharedSettings.FirstOrDefault().ContentTypesWithHreflang
                    : this.contentTypeSettings.Value;

                foreach (var contentType in listOfContentType)
                {
                    try
                    {

                        var gpocContentsEntry =
                            (await repository.GetAllGpocEntry(market.Language, contentType)).ToList();

                        var filteredListOfContent = FilterDuplicates(gpocContentsEntry, market);

                        foreach (var content in filteredListOfContent)
                        {
                            try
                            {
                                await this.ProcessItem(content, market, repository, domainUrl);
                            }
                            catch (Exception ex)
                            {
                                errorList.Add(
                                    $"Error while processing item : {content.SystemProperties.Id} market {market.Language}: {DateTime.UtcNow} - {ex.Message}");

                                logger.LogError(
                                    $"Error while processing item : id : {content.SystemProperties.Id} market {market.Language}- {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");

                            }
                        }
                    }
                    catch (ContentfulException ce)
                    {
                        if (ce.ErrorDetails != null && ce.ErrorDetails.Errors != null)
                        {
                            this.logger.LogWarning(
                                $"Error while processing Content type : {contentType} market {market.Language}- {DateTime.UtcNow} - {ce.Message} - {ce.StackTrace}");
                        }
                        else
                        {
                            this.logger.LogError(
                                $"Error while processing Content type : {contentType} market {market.Language}- {DateTime.UtcNow} - {ce.Message} - {ce.StackTrace}");

                        }
                    }
                    catch (Exception ex)
                    {
                        var exceptionName = ex.GetType().FullName;
                        errorList.Add(
                            $"Error while processing Content type : {contentType} market {market.Language}- {exceptionName} {DateTime.UtcNow} - {ex.Message}");
                        this.logger.LogError(
                            $"Error while processing Content type : {contentType} market {market.Language}- {exceptionName} {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                errorList.Add($"Error during market process : {market.Language} - {ex.Message} - {DateTime.UtcNow}");
                this.logger.LogError($"Error during market process : {market.Language} - {DateTime.UtcNow} - {ex.Message} - {ex.StackTrace}");
            }
            this.logger.LogWarning($"Href lang Ended for : market {market.Language} - {market.SpaceId}");

            return errorList;
        }


        #endregion
    }
}
