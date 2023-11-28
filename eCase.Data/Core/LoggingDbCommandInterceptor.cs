using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace eCase.Data.Core
{
    public class LoggingDbCommandInterceptor : IDbCommandInterceptor
    {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.NonQueryExecuted(command, interceptionContext);
            }
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.NonQueryExecuting(command, interceptionContext);
            }
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.ReaderExecuted(command, interceptionContext);
            }
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.ReaderExecuting(command, interceptionContext);
            }
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.ScalarExecuted(command, interceptionContext);
            }
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts.OfType<LoggingDbContext>())
            {
                dbContext.ScalarExecuting(command, interceptionContext);
            }
        }
    }
}
