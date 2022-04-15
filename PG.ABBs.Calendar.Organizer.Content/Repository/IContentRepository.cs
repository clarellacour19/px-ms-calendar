// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentRepository.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Inteface to the content repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Content.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Contentful.Core.Models;

    public interface IContentRepository
    {
        #region Public Properties

        /// <summary>
        ///     Gets the ProviderName.
        /// </summary>
        string ProviderName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The GetAll.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <returns>The
        ///     <see>
        ///         <cref>Task{IEnumerable{T}}</cref>
        ///     </see>
        /// .</returns>
        Task<IEnumerable<T>> GetAll<T>(string locale, string contentType) where T : class;

        Task<IEnumerable<T>> GetAllShared<T>(string locale, string contentType,string sharedLocale,string defaultLocale) where T : class;

        Task<IEnumerable<T>> GetShared<T>(string locale, string contentType, string sharedLocale) where T : class;

        /// <summary>
        ///     The GetAll.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <returns>The
        ///     <see>
        ///         <cref>Task{IEnumerable{T}}</cref>
        ///     </see>
        /// .</returns>
        Task<IEnumerable<T>> GetAllGpoc<T>(string locale, string contentType) where T : class;

        /// <summary>
        ///     The GetAllGpocEntry by content type.
        /// </summary>
        /// <returns>The
        ///     <see>
        ///         <cref>Task{IEnumerable{Entry{dynamic}}}</cref>
        ///     </see>
        /// .</returns>
        Task<IEnumerable<Entry<dynamic>>> GetAllGpocEntry(string locale, string contentType);

        /// <typeparam name="T"></typeparam>
        /// <param name="entryId"></param>
        /// <param name="locale"></param>
        /// <param name="specifyType"></param>
        /// <returns>The
        ///     <see>
        ///         <cref>Task{IEnumerable{T}}</cref>
        ///     </see>
        ///     .</returns>
        Task<IEnumerable<T>> GetRelatedEntries<T>(string entryId, string locale, bool specifyType = true) where T : class;

        /// <summary>
        ///     The Single.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="entryId">The entryId<see cref="string" />.</param>
        /// <param name="locale"></param>
        /// <returns>The <see cref="Task{T}" />.</returns>
        Task<T> Single<T>(string entryId, string locale) where T : class;

        /// <summary>
        ///     The Find.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="value"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Find<T>(Expression<Func<T, object>> where, string value, string locale) where T : class;

        /// <summary>
        ///     The Update.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        Task<Entry<dynamic>> Update(Entry<dynamic> entry, string locale);

        #endregion
    }
}