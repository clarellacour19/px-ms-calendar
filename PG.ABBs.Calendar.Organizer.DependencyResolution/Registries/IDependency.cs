// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependency.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   IDependency interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.DependencyResolution.Registries
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public interface IDependency
    {
        #region Public Methods and Operators

        void Register(IServiceCollection services, IConfiguration configuration, bool isDevelopment);

        #endregion
    }
}
