using Epep.MobileApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddDbContext();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c => {
//    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
//    c.IgnoreObsoleteActions();
//    c.IgnoreObsoleteProperties();
//    c.CustomSchemaIds(type => type.FullName);
//});

builder.AddApplicationAuthentication();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureIOServices(builder.Configuration);
builder.Services.ConfigureHttpClients(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowAll",
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();



app.Run();
