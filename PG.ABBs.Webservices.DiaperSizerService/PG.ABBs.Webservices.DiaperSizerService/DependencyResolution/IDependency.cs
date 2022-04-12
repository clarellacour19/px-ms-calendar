
using Microsoft.Extensions.DependencyInjection;

namespace PG.ABBs.Webservices.DiaperSizerService.DependencyResolution
{
    interface IDependency
    {
        void Register(IServiceCollection services);
    }
}
