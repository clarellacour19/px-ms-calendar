﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleRegistration.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Register all service dependencies.
// </summary>
// -------------------------------------------------------------------------------------------------------------------- 

using PG.ABBs.Calendar.Organizer.Content.GraphQL;
using PG.ABBs.Calendar.Organizer.Content.RetryPolicy;

namespace PG.ABBs.Calendar.Organizer.Service.Dependency
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using PG.ABBs.Calendar.Organizer.Content;
    using PG.ABBs.Calendar.Organizer.Content.Configuration;
    using PG.ABBs.Calendar.Organizer.Content.Repository;
    using PG.ABBs.Calendar.Organizer.Data.Context;
    using PG.ABBs.Calendar.Organizer.Data.Repositories;
    using PG.ABBs.Calendar.Organizer.Data.Constant;
    using PG.ABBs.Calendar.Organizer.DependencyResolution.Registries;
    using PG.ABBs.Calendar.Organizer.Service.Services;
    using  PG.ABBs.Calendar.Organizer.AzureStorage;

    public class ModuleRegistration : IDependency
    {
        #region Public Methods and Operators

        public void Register(IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            //services.Configure<CiamProviders>(configuration.GetSection("CiamProviders"));

            //services.AddScoped(config => config.GetService<IOptionsSnapshot<DatabaseSettings>>().Value);
            //services.AddScoped(config => config.GetService<IOptionsSnapshot<CiamProviders>>().Value);

            services.Configure<SharedMarketSettings>(configuration.GetSection(SharedMarketSettings.SectionName));

            services.Configure<List<MarketSettings>>(configuration.GetSection(MarketSettings.SectionName));
            //azure storage account
            services.Configure<StorageModel>(configuration.GetSection(StorageModel.SectionName));

            if (configuration["KeyVault:Vault"] != null
                && configuration["KeyVault:ClientId"] != null
                && configuration["KeyVault:ClientSecret"] != null)
            {

                if (isDevelopment)
                {
                    services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration[Constant.ConnectionString]));
                }
                else
                {
                    services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration[Constant.KeyVaultConnectionString]));
                }
                services.PostConfigure<List<MarketSettings>>(marketOptions =>
                {
                    foreach (var market in marketOptions)
                    {
                        market.PreviewApiKey = configuration[ $"{Constant.KeyVaultPreviewApiKey}-{market.SpaceId}"];
                        market.DeliveryApiKey = configuration[$"{Constant.KeyVaultDeliveryApiKey}-{market.SpaceId}"];
                        market.ManagementApiKey = configuration[Constant.KeyVaultManagementApiKey];
                    }
                });

                services.PostConfigure<SharedMarketSettings>(settings =>
                {
                    settings.DeliveryApiKey = configuration[$"{Constant.KeyVaultDeliveryApiKey}-{settings.SpaceId}"];
                    settings.PreviewApiKey = configuration[$"{Constant.KeyVaultPreviewApiKey}-{settings.SpaceId}"];
                    settings.ManagementApiKey = configuration[$"{Constant.KeyVaultManagementApiKey}"];

                    if(string.IsNullOrEmpty(settings.DeliveryApiKey) || string.IsNullOrEmpty(settings.PreviewApiKey))
                    {
                        settings.DeliveryApiKey = configuration[$"SharedMarket:DeliveryApiKey"];
                        settings.PreviewApiKey = configuration[$"SharedMarket:PreviewApiKey"];
                        settings.ManagementApiKey = configuration[$"{Constant.KeyVaultManagementApiKey}"];
                    }
                });

            }
            else
            {
                services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration[Constant.ConnectionString]));
            }
            
            
            services.AddScoped(typeof(IDataRepository<>), typeof(DatabaseRepository<>));
            services.AddScoped<IUnitOfWork<DataContext>, UnitOfWork<DataContext>>();
            services.AddScoped<IHrefLangService, HrefLangService>();

            services.Configure<ContentProviderSettings>(configuration.GetSection(ContentProviderSettings.SectionName));
            services.AddScoped(config => config.GetService<IOptionsSnapshot<ContentProviderSettings>>().Value);

            //services.Configure<List<string>>(configuration.GetSection(ContentSettings.GpocContentTypeSectionName));
            // services.Configure<ContentSettings>(configuration.GetSection(ContentSettings.SectionName));
            // services.AddScoped(config => config.GetService<IOptionsSnapshot<ContentSettings>>().Value);
            services.AddScoped<IContentRepository, DefaultRepository>();
            services.AddScoped<IRetryPolicy, RetryPolicyService>();
            services.AddTransient<ClientFactory>();
            services.AddTransient<ContentManager>();
            services.AddSingleton<MarketSettingsHelper>();
            services.AddTransient<StorageClient>();
        }

        #endregion
    }
}