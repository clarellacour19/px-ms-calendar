// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   UnitOfWork interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Repositories
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        #region Constructors and Destructors

        public UnitOfWork(TDbContext context)
        {
            this.Context = context;
        }

        #endregion

        #region Public Properties

        public TDbContext Context { get; }

        #endregion

        #region Public Methods and Operators

        public int Commit()
        {
            return this.Context.SaveChanges();
        }

        public async Task<int> CommitChangesAsync()
        {
            return await this.Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IDataRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            return new DatabaseRepository<TEntity>(new UnitOfWork<DbContext>(this.Context));
        }

        #endregion

        #region Explicit Interface Methods

        int IDisposableUnitOfWork.Commit()
        {
            return this.Context.SaveChanges();
        }

        #endregion
    }
}
