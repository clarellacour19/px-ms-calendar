// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRepository.cs" company="The Procter & Gamble Company">
//   Copyright © The Procter & Gamble Company 2020 - All Rights Reserved.
// </copyright>
// <summary>
//   DatabaseRepository implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PG.ABBs.Calendar.Organizer.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using StoredProcedureEFCore;

    public class DatabaseRepository<T> : IDataRepository<T>
        where T : class
    {
        #region Fields

        private readonly IUnitOfWork<DbContext> unitOfWork;

        #endregion

        #region Constructors and Destructors

        public DatabaseRepository(IUnitOfWork<DbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion

        #region Public Methods and Operators

        public void Add(T entity)
        {
            this.unitOfWork.Context.Set<T>().Add(entity);
        }

        public void Add(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<T> entities)
        {
            this.unitOfWork.Context.Set<T>().AddRange(entities);
        }

        public void Delete(T entity)
        {
            var existing = this.unitOfWork.Context.Set<T>().Find(entity);
            if (existing != null)
                this.unitOfWork.Context.Set<T>().Remove(existing);
        }

        public void Delete(object id)
        {
            var existing = this.unitOfWork.Context.Set<T>().Find(id);
            if (existing != null)
                this.unitOfWork.Context.Set<T>().Remove(existing);
        }

        public void Delete(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<T> entities)
        {
            this.unitOfWork.Context.Set<T>().RemoveRange(entities);
        }

        public void Dispose()
        {
            this.unitOfWork.Context.Dispose();
        }

        public IList<T> ExecuteStoredProcedure(string procedure, IDictionary<string, object> parameters)
        {
            IList<SqlParameter> args = new List<SqlParameter>();
            var command = $" {procedure} ";
            if (parameters != null)
            {
                var i = 0;
                foreach (var key in parameters.Keys)
                {
                    args.Add(new SqlParameter($"@{key}", parameters[key]));
                    command += i != parameters.Count - 1 ? $" @{key} , " : $" @{key} ";
                    i++;
                }

                return this.unitOfWork.Context.Set<T>().FromSqlRaw($"{command}", args.ToArray()).ToList();
            }

            return this.unitOfWork.Context.Set<T>().FromSqlRaw($"{command}").ToList();
        }

        public void ExecuteNonQueryStoredProcedure(string procedureName, IDictionary<string, object> parameters)
        {
            var builder = this.unitOfWork.Context.LoadStoredProc(procedureName);
            foreach (var param in parameters)
            {
                var sqlParam = new SqlParameter(param.Key, param.Value);
                builder.AddParam(sqlParam);
            }

            builder.ExecNonQuery();
        }

        public ValueTask<T> FindById(object id)
        {
            return this.unitOfWork.Context.Set<T>().FindAsync(id);
        }

        public async Task<T> GetSingleWithInclude(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = this.unitOfWork.Context.Set<T>();

            //Apply eager loading
            foreach (var navigationProperty in navigationProperties)
                query = query.Include(navigationProperty);

            var item = await query.AsNoTracking().SingleOrDefaultAsync(where);

            return item;
        }

        public async Task<T> GetSingleWithGenericInclude<TResult>(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<TResult, object>> include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = this.unitOfWork.Context.Set<T>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = (IQueryable<T>)include(query);
            }

            var item = await query.FirstOrDefaultAsync(predicate);

            return item;
        }

        public IEnumerable<T> Get()
        {
            return this.unitOfWork.Context.Set<T>().AsEnumerable();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return this.unitOfWork.Context.Set<T>().Where(predicate).AsEnumerable();
        }

        public Task<List<T>> GetAll()
        {
            return this.unitOfWork.Context.Set<T>().ToListAsync();
        }

        public Task<List<T>> GetAllWithInclude<TResult>(Func<IQueryable<T>, IIncludableQueryable<TResult, object>> include = null, bool disableTracking = true)
        {
            IQueryable<T> query = this.unitOfWork.Context.Set<T>();
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = (IQueryable<T>)include(query);
            }

            return query.ToListAsync();
        }

        public IQueryable<T> Query(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public T Search(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public T Single(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            this.unitOfWork.Context.Set<T>().Attach(entity);
            this.unitOfWork.Context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(params T[] entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities)
        {
            this.unitOfWork.Context.UpdateRange(entities);
        }

        #endregion
    }
}
