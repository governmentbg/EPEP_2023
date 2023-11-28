using eCase.Common.NLog;
using System;
using System.Threading;
using System.Web.Hosting;

namespace eCase.Common.Jobs
{
    public class JobHost : IDisposable
    {
        private readonly object jobLock = new object();

        private IJob job;
        private ILogger logger;
        private Timer timer;

        public JobHost(ILogger logger)
        {
            this.logger = logger;
        }

        // reads & writes of bool are atomic, so no lock is required
        public bool IsShuttingDown { get; private set; }

        public void Start(IJob job, CancellationToken token)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            this.job = job;
            this.logger.Info(string.Format("Initializing {0}", this.job.Name));
            this.timer = new Timer((sender) => this.DoAction(sender, token));
            this.timer.Change(TimeSpan.FromSeconds(0), this.job.Period);
        }

        private void DoAction(object sender, CancellationToken token)
        {
            if (this.IsShuttingDown)
            {
                return;
            }

            // DoAction returns immediately if the previous action has not finished
            if (Monitor.TryEnter(this.jobLock))
            {
                try
                {
                    this.job.Action(token);
                }
                catch (Exception e)
                {
                    this.logger.Error(string.Format("Error while running {0} {1}", this.job.Name, Helpers.Helper.GetDetailedExceptionInfo(e)));
                }
                finally
                {
                    Monitor.Exit(this.jobLock);
                }
            }
        }

        public void Dispose()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            if (this.job != null)
            {
                this.job.Dispose();
                this.logger.Info(string.Format("Disposed {0}", this.job.Name));
                this.job = null;
            }
        }
    }
}