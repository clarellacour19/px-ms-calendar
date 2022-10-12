using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
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
//Api Versioning

// Updating the middlewear to use versioning.
services.AddApiVersioning(setup =>
{
	setup.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
	setup.AssumeDefaultVersionWhenUnspecified = true;
	setup.ReportApiVersions = true;
	setup.ApiVersionReader = new UrlSegmentApiVersionReader();

});
services.AddVersionedApiExplorer(setup =>
{
	setup.GroupNameFormat = "'v'VVV";
	setup.SubstituteApiVersionInUrl = true;
});

services.AddSwaggerGen();

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

	var versionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();

	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		//options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
		foreach (var description in versionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
		}
	});


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