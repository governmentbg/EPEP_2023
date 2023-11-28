using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace IO.QuartzExtensions
{
    public class QuartzJobInfo
    {
        public string Description { get; set; }
        public string TypeName { get; set; }
        public string CronExpression { get; set; }
        public int? FetchCount { get; set; }
        public bool Disabled { get; set; }
    }
    public static class BaseQuartzExtensions
    {
        public static void AddQuartConfiguration(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddQuartz(q =>
            {
                q.ConfigureQuartzServices(Configuration);
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.AwaitApplicationStarted = true;
                options.WaitForJobsToComplete = true;
            });
        }


        static void ConfigureQuartzServices(this IServiceCollectionQuartzConfigurator service, IConfiguration Configuration)
        {
            service.UseMicrosoftDependencyInjectionJobFactory();

            var quartzJobs = Configuration.GetSection("QuartzJobs").Get<List<QuartzJobInfo>>();
            if (quartzJobs == null)
            {
                return;
            }
            string jobGroup = "statGroup";

            foreach (var qJob in quartzJobs)
            {
                if (qJob.Disabled)
                {
                    continue;
                }
                try
                {
                    Type jobType = Type.GetType(qJob.TypeName);
                    var job1Key = new JobKey(Guid.NewGuid().ToString(), jobGroup);
                    service.AddJob(jobType, job1Key, a =>
                    {
                        a.WithDescription(qJob.Description);
                        if (qJob.FetchCount > 0)
                        {
                            a.UsingJobData("fetchCount", qJob.FetchCount.Value);
                        }
                    });

                    service.AddTrigger(t => t
                       .ForJob(job1Key)
                       .StartNow()
                       .WithCronSchedule(qJob.CronExpression)
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception($"QUARTZ init Error!, job:{qJob.TypeName}; Exception:{ex.Message}");
                }
            }
        }
        public class BaseJob : IJob
        {
            internal ILogger<BaseJob> logger;

            protected virtual async Task DoJob(IJobExecutionContext context) { }
            public async Task Execute(IJobExecutionContext context)
            {
                QuartzLog(context, "started.");
                await DoJob(context).ConfigureAwait(false);
                QuartzLog(context, "finished.");
            }

            private void QuartzLog(IJobExecutionContext context, string message = "")
            {
                logger.LogWarning($"{context.JobDetail.Description} {message}");
            }
        }
        /*
"QuartzJobs": [
    {
      "Description": "ЕЕСПП",
      "TypeName": "QuartzTest.MockJob",
      "CronExpression": "0 0 8/2 ? * * *",
      "ExpressionInfo": "Всеки втори час започвайки от 08:00, всеки ден"
    },
    {
      "Description": "СИСМА",
      "TypeName": "QuartzTest.Mock2Job",
      "CronExpression": "0/2 * * ? * * *"
    }
  ]

        */
    }
}
