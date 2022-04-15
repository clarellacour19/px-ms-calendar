// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentManager.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Content manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PG.ABBs.Calendar.Organizer.Content.Configuration;
    using PG.ABBs.Calendar.Organizer.Content.Repository;

    public class ContentManager
    {
        #region Fields

        /// <summary>
        ///     Defines the currentRepository.
        /// </summary>
        private readonly IContentRepository currentRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentManager" /> class.
        /// </summary>
        /// <param name="repositories">The repositories<see cref="IEnumerable{T}" />.</param>
        /// <param name="settings">The settings<see cref="ContentSettings" />.</param>
        public ContentManager(IEnumerable<IContentRepository> repositories, ContentProviderSettings settings)
        {
            var contentRepositories = repositories as IContentRepository[] ?? repositories.ToArray();

            if (!contentRepositories.Any() || !contentRepositories.Any(p =>
                p.ProviderName.Equals(settings.ProviderName, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception($"No processors defined for Security Provider {settings.ProviderName}");
            }

            this.currentRepository = contentRepositories.FirstOrDefault(x =>
                x.ProviderName.Equals(settings.ProviderName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IContentRepository GetRepository()
        {
            return this.currentRepository;
        }

        #endregion
    }
}