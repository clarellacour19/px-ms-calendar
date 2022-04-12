using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PG.ABBs.Webservices.DiaperSizerService.Repositories;
using PG.ABBs.Webservices.DiaperSizerService.Services;
using PG.ABBs.Webservices.DiaperSizerService.Settings;

namespace PG.ABBs.Webservices.DiaperSizerService.DependencyResolution
{
    public class DependencyRegistry : IDependency
    {
        public void Register(IServiceCollection services)
        {
            #region Settings

            services.AddScoped(configuration => configuration.GetService<IOptionsSnapshot<DatabaseSettings>>().Value);

            #endregion

            #region Services and Repositories

            services.AddScoped<IDiaperSizerServices, DiaperSizerServices>();
            services.AddScoped<IDiaperSizerRepository, DiaperSizerRepository>();

            #endregion
        }
    }
}
