using SftpPolling.BackgroundServices;
using SftpPolling.Configuration;
using SftpPolling.Configuration.AppsettingsOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (builder.Environment.IsProduction())
{
    builder.RegisterAzureAppConfiguration();
    builder.RegisterAppInsights();
}

builder.Services.Configure<SftpsOptions>(builder.Configuration.GetSection(SftpsOptions.SectionName));
builder.Services.AddHostedService<SftpBackgroundService>();

var app = builder.Build();

if (builder.Environment.IsProduction())
{
    app.UseAzureAppConfiguration();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();