using Azure.Identity;

namespace SftpPolling.Configuration;

public static class AzureAppConfigurationRegistration
{
    public static void RegisterAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        const string appConfigEndpointKey = "AppConfigurationEndpoint";
        var appConfigEndpointStr = builder.Configuration.GetValue<string>(appConfigEndpointKey);
        if (string.IsNullOrWhiteSpace(appConfigEndpointStr))
        {
            throw new InvalidOperationException($"'{appConfigEndpointKey}' configuration key not found");
        }
    
        builder.Configuration.AddAzureAppConfiguration(config =>
        {
            var endpoint = new Uri(appConfigEndpointStr);
            config.Connect(endpoint, new ManagedIdentityCredential());
        });
        
        builder.Services.AddAzureAppConfiguration();
    }
}