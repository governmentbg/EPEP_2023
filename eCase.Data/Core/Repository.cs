using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace eCase.Data.Core
{
    internal class Repository : IRepository
    {
        protected UnitOfWork unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = (UnitOfWork)unitOfWork;
        }

        public void LoadReference<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class
        {
            this.unitOfWork.DbContext.Entry(entity).Reference(navigationProperty).Load();
        }

        #region Protected methods

        protected virtual void ExecuteSqlCommand(string sql, List<SqlParameter> parameters)
        {
            this.unitOfWork.DbContext.Database.ExecuteSqlCommand(sql, parameters.ToArray());
        }

        protected DbRawSqlQuery<TSpEntity> ExecProcedure<TSpEntity>(string procedureName, List<SqlParameter> parameters)
        {
            StringBuilder sb = new StringBuilder(procedureName + " ");

            for (int i = 0; i < parameters.Count; i++)
            {
                sb.AppendFormat("@{0}", parameters[i].ParameterName);
                if (i != parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return this.unitOfWork.DbContext.Database.SqlQuery<TSpEntity>(
                sb.ToString(),
                parameters.ToArray());
        }

        protected List<TSpEntity> ExecFunction<TSpEntity>(string procedureName, List<SqlParameter> parameters)
        {
            StringBuilder sb = new StringBuilder("SELECT * FROM " + procedureName + "(");

            for (int i = 0; i < parameters.Count; i++)
            {
                sb.AppendFormat("@{0}", parameters[i].ParameterName);
                if (i != parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");

            return this.unitOfWork.DbContext.Database.SqlQuery<TSpEntity>(
                sb.ToString(),
                parameters.ToArray()).ToList();
        }

        protected DbRawSqlQuery<TSpEntity> SqlQuery<TSpEntity>(string sql, List<SqlParameter> parameters)
        {
            return this.unitOfWork.DbContext.Database.SqlQuery<TSpEntity>(sql, parameters.ToArray());
        }

        #endregion
    }
}
