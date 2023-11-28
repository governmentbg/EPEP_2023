using eCase.Common.NLog;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Text;

namespace eCase.Data.Core
{
    internal class LoggingDbContext : DbContext, IDbCommandInterceptor
    {
        private ILogger logger;
        private Stopwatch sw = new Stopwatch();

        public LoggingDbContext(
            ILogger logger,
            string nameOrConnectionString,
            DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.logger = logger;
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            this.CommandExecuted(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            this.CommandExecuting(command, interceptionContext);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.CommandExecuted(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            this.CommandExecuting(command, interceptionContext);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.CommandExecuted(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            this.CommandExecuting(command, interceptionContext);
        }

        private void CommandExecuting<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            if (this.sw.IsRunning)
            {
                this.logger.Warn("Executing more than one queries/commands at the same time for a single DbContext.");
            }

            this.sw.Restart();
        }

        private void CommandExecuted<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            this.sw.Stop();

            if (this.sw.Elapsed.Milliseconds >= 500)
            {
                this.logger.Warn("Slow query!\n" + this.CreateCommandText(command, sw.Elapsed.Milliseconds));
            }
        }

        private string CreateCommandText(DbCommand command, int ms)
        {
            StringBuilder commandText = new StringBuilder();

            commandText.AppendFormat("Query executed in {0}ms\n", ms);

            commandText.AppendLine(command.CommandText);

            foreach (DbParameter param in command.Parameters)
            {
                commandText.AppendFormat("@{0} = {1}\n", param.ParameterName, param.Value.ToString());
            }

            return commandText.ToString();
        }
    }
}
