// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyManager.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   DependencyManager implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.DependencyResolution
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public sealed class DependencyManager
    {
        #region Static Fields

        private static readonly Lazy<DependencyManager> Instance =
            new Lazy<DependencyManager>(() => new DependencyManager(), LazyThreadSafetyMode.ExecutionAndPublication);

        #endregion

        #region Constructors and Destructors

        private DependencyManager()
        {
        }

        #endregion

        #region Public Properties

        public static DependencyManager DependencyInstance => Instance.Value;

        #endregion

        #region Public Methods and Operators

        public IEnumerable<Assembly> GetCustomAssemblies()
        {
	        try
	        {
		        var dlls = Directory.GetFiles(AppContext.BaseDirectory).Where(
			        x => !string.IsNullOrEmpty(x) && Path.GetFileName(x).StartsWith("PG")
			                                      && Path.GetFileName(x).EndsWith(".dll"));

		        return dlls.Select(dll => Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(dll)))).ToList();
            }
	        catch (Exception e)
	        {
			        Console.WriteLine(e);
			        throw;
	        }
            
        }

        #endregion
    }
}
