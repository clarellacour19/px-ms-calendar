using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PG.ABBs.Calendar.Organizer.Service.Swagger
{
	public class ConfigureSwaggerOptions : Microsoft.Extensions.Options.IConfigureNamedOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider provider;

		public ConfigureSwaggerOptions(
			IApiVersionDescriptionProvider provider)
		{
			this.provider = provider;
		}

		public void Configure(SwaggerGenOptions options)
		{
			// add swagger document for every API version discovered
			foreach (var description in provider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(
					description.GroupName,
					CreateVersionInfo(description));
			}
		}

		public void Configure(string name, SwaggerGenOptions options)
		{
			Configure(options);
		}

		private OpenApiInfo CreateVersionInfo(
			ApiVersionDescription description)
		{
			var info = new OpenApiInfo()
			{
				Title = "Autopublish Batch API",
				Version = description.ApiVersion.ToString()
			};

			if (description.IsDeprecated)
			{
				info.Description += " This API version has been deprecated.";
			}

			return info;
		}
	}
}
