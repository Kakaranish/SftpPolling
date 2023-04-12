namespace SftpPolling.Configuration;

public static class AzureAppInsightsRegistration
{
    public static void RegisterAppInsights(this WebApplicationBuilder builder)
    {
        const string appInsightsConnectionStringKey = "AppInsightsConnectionString";
        var appInsightsConnectionStr = builder.Configuration.GetValue<string>(appInsightsConnectionStringKey);
        if (string.IsNullOrWhiteSpace(appInsightsConnectionStr))
        {
            throw new InvalidOperationException($"'{appInsightsConnectionStringKey}' configuration key not found");
        }
        
        builder.Services.AddApplicationInsightsTelemetry(appInsightsBuilder =>
            {
                appInsightsBuilder.ConnectionString = appInsightsConnectionStr;
            }
        );
    }
}