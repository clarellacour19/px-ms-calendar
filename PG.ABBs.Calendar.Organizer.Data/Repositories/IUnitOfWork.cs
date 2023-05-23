// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   IUnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;

    public interface IUnitOfWork<out TDbContext> : IDisposableUnitOfWork
        where TDbContext : DbContext
    {
        #region Public Properties

        TDbContext Context { get; }

        #endregion
    }
}
