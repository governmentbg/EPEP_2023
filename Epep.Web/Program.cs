using Epep.Core.Constants;
using Epep.Web.Extensions;
using Epep.Web.Extensions.ModelBinders;
using Rotativa.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("hosting.json", true, true).AddEnvironmentVariables("ASPNETCORE_");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("logs/epep-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


builder.AddDbContext();

// Add services to the container.
builder.Services.AddControllersWithViews().AddMvcOptions(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider(NomenclatureConstants.NormalDateFormat));
    //options.ModelBinderProviders.Insert(1, new DoubleModelBinderProvider());
    options.ModelBinderProviders.Insert(1, new DecimalModelBinderProvider());
});
builder.Services.AddRazorPages();
builder.AddApplicationAuthentication();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureIOServices(builder.Configuration);
builder.Services.ConfigureHttpClients(builder.Configuration);

//if (builder.Environment.IsDevelopment() && false)
//    builder.Services.Configure<KestrelServerOptions>(options =>
//    {
//        options.ConfigureHttpsDefaults(options =>
//        options.ClientCertificateMode = ClientCertificateMode.AllowCertificate);
//    });

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

app.UseCertificateForwarding();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use((authContext, next) =>
{
    authContext.Request.Scheme = "https";
    return next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();
RotativaConfiguration.Setup(app.Environment.WebRootPath, builder.Configuration.GetValue<string>("RotativaLibRelativePath"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{gid?}");

app.Run();
