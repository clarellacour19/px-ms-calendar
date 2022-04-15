// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHrefLangService.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   Interface for Hreflang service
// </summary>
// -------------------------------------------------------------------------------------------------------------------- 

using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using PG.ABBs.Calendar.Organizer.Content.Domain;

using System.Collections.Generic;
using Contentful.Core.Models;
using PG.ABBs.Calendar.Organizer.Content.Configuration;

namespace PG.ABBs.Calendar.Organizer.Service.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IHrefLangService
    {
        #region Methods
        Task<Object> Process();

        void DeleteEntry (DeletedEntity entry);

        List<Entry<dynamic>> FilterDuplicates(List<Entry<dynamic>> gpocContentList, MarketSettings market);
        Task<Object> ProcessByMarket(string sitename);

        #endregion
    }
}
