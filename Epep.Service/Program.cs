using Epep.Service.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;

var host = BuildHost(args);

var logPath = "logs/epep-log.txt";
if (OperatingSystem.IsWindows())
{
    logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
    logPath += @"\logs\epep-log.txt";
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

var hostBuilder = host.Build();
hostBuilder.Run();
Console.WriteLine("Started");
Console.ReadLine();

static IHostBuilder BuildHost(string[] args)
{
    return Host.CreateDefaultBuilder()
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.SetBasePath(Directory.GetCurrentDirectory());
                        configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                        configHost.AddCommandLine(args);
                    })
                 .ConfigureAppConfiguration((hostContext, configApp) =>
                 {
                     configApp.SetBasePath(Directory.GetCurrentDirectory());
                     configApp.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                     configApp.AddJsonFile($"appsettings.json", true);
                     configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
                     configApp.AddCommandLine(args);
                 })
                 .ConfigureLogging((hostingContext, logBuilder) =>
                 {
                     logBuilder.ClearProviders();
                     logBuilder.AddSerilog();
                     //if (OperatingSystem.IsWindows())
                     //{
                     //    logBuilder.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                     //}

                     //logBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     //logBuilder.AddEventLog();
                     //logBuilder.AddConsole();
                     //hostingContext.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     //configLogging.AddConsole();
                     //configLogging.AddDebug();
                 })

                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddDbContext(hostContext.Configuration);
                     services.ConfigureHttpClients(hostContext.Configuration);
                     services.ConfigureServices(hostContext.Configuration);

                     //if (OperatingSystem.IsWindows())
                     //{
                     //    services.Configure<EventLogSettings>(config => {
                     //        config.LogName = "Epep Service";
                     //        //config.SourceName = "Epep Service Source";
                     //    });
                     //}
                 });
}