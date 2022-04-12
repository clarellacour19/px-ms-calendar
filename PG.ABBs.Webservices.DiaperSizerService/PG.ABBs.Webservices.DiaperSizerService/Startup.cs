using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using PG.ABBs.Webservices.DiaperSizerService.Context;
using PG.ABBs.Webservices.DiaperSizerService.DependencyResolution;
using PG.ABBs.Webservices.DiaperSizerService.Settings;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;

namespace PG.ABBs.Webservices.DiaperSizerService
{
    public class Startup
    {
        private static string _authority;
        private static string _audience;
        private static string _clientSecret;
        private static string _clientID;
        private static string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<DatabaseSettings>(Configuration.GetSection(Constants.DatabaseSettings));

            services.AddRazorPages();

            services.AddMvcCore().AddApiExplorer();

            services.AddControllers();

            services.AddCors(o =>
                o.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.Authority = _authority;
                     options.Audience = _audience;
                 });

            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(_connectionString));

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Diaper Sizer API", Version = "V1" });
            });

            DependencyManager.DependencyInstance.BuildDependencies(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _authority = Configuration[Constants.Authority];
            _audience = Configuration[Constants.Audience];
            _clientSecret = Configuration[Constants.ClientSecret];
            _clientID = Configuration[Constants.ClientID];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            _connectionString = Configuration[Constants.ConnectionString];

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.Use(async (context, next) =>
            {
                IConfidentialClientApplication app;

                app = ConfidentialClientApplicationBuilder.Create(_clientID)
                    .WithClientSecret(_clientSecret)
                    .WithAuthority(new Uri(_authority))
                    .Build();

                string[] ResourceIds = new string[] { $"{_audience}/.default" };

                AuthenticationResult result = null;
                try
                {
                    result = app.AcquireTokenForClient(ResourceIds).ExecuteAsync().GetAwaiter().GetResult();
                    var token = result.AccessToken;
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Request.Headers.Add("Authorization", "Bearer " + token);
                    }
                    await next();
                }
                catch (Exception ex)
                {
                    if (ex is MsalClientException || ex is MsalServiceException)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync($"Not Authorized {ex.Message}");
                    }
                    else throw;
                }
            });
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c => {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => 
                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}" } });
            });

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "post API V1");
            });
        }
    }
}
