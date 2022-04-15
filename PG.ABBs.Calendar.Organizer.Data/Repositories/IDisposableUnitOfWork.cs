// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposableUnitOfWork.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   IDisposableUnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Repositories
{
    using System;
    using System.Threading.Tasks;

    public interface IDisposableUnitOfWork : IDisposable
    {
        #region Public Methods and Operators

        int Commit();

        Task<int> CommitChangesAsync();

        IDataRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        #endregion
    }
}
