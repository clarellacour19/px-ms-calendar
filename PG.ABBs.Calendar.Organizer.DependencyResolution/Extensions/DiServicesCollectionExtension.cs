// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiServicesCollectionExtension.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   DiServicesCollectionExtension implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.DependencyResolution.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PG.ABBs.Calendar.Organizer.DependencyResolution.Registries;

    public static class DiServicesCollectionExtension
    {
        #region Public Methods and Operators

        public static void BuildDependencies(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            var type = typeof(IDependency);

            var assemblies = DependencyManager.DependencyInstance.GetCustomAssemblies();

            var instances = new List<IDependency>();
            foreach (var assembly in assemblies) { 
                instances.AddRange(
                    assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && t.GetTypeInfo().IsClass)
                        .Select(t => Activator.CreateInstance(t) as IDependency).ToList());
            }

            instances.ForEach(x => {
                x.Register(services, configuration, isDevelopment);
                }
            );

        }

        #endregion
    }
}
