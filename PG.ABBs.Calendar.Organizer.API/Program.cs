using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Models;
using PG.ABBs.Calendar.Organizer.API;
using PG.ABBs.Calendar.Organizer.DependencyResolution.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.Configuration;
var services = builder.Services;
if (config["KeyVault:Vault"] != null
    && config["KeyVault:ClientId"] != null
    && config["KeyVault:ClientSecret"] != null)
{
	config.AddAzureKeyVault(config["KeyVault:Vault"], config["KeyVault:ClientId"], config["KeyVault:ClientSecret"]);
}

var _authority = config[Constants.Authority];
var _audience = config[Constants.Audience];
var _clientSecret = config[Constants.ClientSecret];
var _clientID = config[Constants.ClientID];

var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
aiOptions.EnableAdaptiveSampling = false;
aiOptions.EnableActiveTelemetryConfigurationSetup = true;
aiOptions.EnableRequestTrackingTelemetryModule = true;
aiOptions.EnableDependencyTrackingTelemetryModule = true;
aiOptions.EnableDiagnosticsTelemetryModule = true;
services.AddApplicationInsightsTelemetry(aiOptions);

services.AddLogging(loggingBuilder =>
{
	loggingBuilder.AddConsole();
	loggingBuilder.AddDebug();
	loggingBuilder.AddApplicationInsights();;
});



services
	.AddMvc(opt => opt.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
services.AddMemoryCache();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.Authority = _authority;
		options.Audience = _audience;
	});
services.AddSwaggerGen(
	options =>
	{
		options.SwaggerDoc(
			"v1",
			new OpenApiInfo
			{
				Title = "Microservice - Calendar Organizer Web HTTP API",
				Version = "v1",
				Description =
					"The Calendar Organizer Microservice HTTP API. This is all microservice API endpoints"
			});
	});

services.AddCors(
	o => o.AddPolicy(
		"AllowSpecificOrigin",
		builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

services.BuildDependencies(config, builder.Environment.IsDevelopment());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
	Trace.TraceInformation($"Using {app.Environment.EnvironmentName} Environment");
	app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Calendar Organizer WebAPI V1"); });

	app.UseDeveloperExceptionPage();
}
else
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseCors(x => x
	.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader());

app.UseHttpsRedirection();


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
app.UseAuthentication();
app.UseMvc();

app.MapControllers();

app.Run();