// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRepository.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   IDataRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore.Query;

    public interface IDataRepository<T> : IDisposable
        where T : class
    {
        #region Public Methods and Operators

        void Add(T entity);

        void Add(params T[] entities);

        void Add(IEnumerable<T> entities);

        void Delete(T entity);

        void Delete(object id);

        void Delete(params T[] entities);

        void Delete(IEnumerable<T> entities);

        IList<T> ExecuteStoredProcedure(string procedure, IDictionary<string, object> parameters);

        void ExecuteNonQueryStoredProcedure(string procedureName, IDictionary<string, object> parameters);

        ValueTask<T> FindById(object id);

        Task<T> GetSingleWithInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);

        Task<T> GetSingleWithGenericInclude<TResult>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<TResult, object>> include = null,
            bool disableTracking = true);

        Task<List<T>> GetAll();

        Task<List<T>> GetAllWithInclude<TResult>(
            Func<IQueryable<T>, IIncludableQueryable<TResult, object>> include = null,
            bool disableTracking = true);

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> Query(string sql, params object[] parameters);

        T Search(params object[] keyValues);

        T Single(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true);

        void Update(T entity);

        void Update(params T[] entities);

        void Update(IEnumerable<T> entities);


        #endregion
    }
}
