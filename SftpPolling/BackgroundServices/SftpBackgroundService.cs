using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Async;
using SftpPolling.Configuration;
using SftpPolling.Configuration.AppsettingsOptions;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace SftpPolling.BackgroundServices;

public class SftpBackgroundService : BackgroundService
{
    private readonly ILogger<SftpBackgroundService> _logger;
    private readonly SftpsOptions _sftpsOptions;
    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(500);

    public SftpBackgroundService(IOptionsMonitor<SftpsOptions> sftpsOptions,
        ILogger<SftpBackgroundService> logger)
    {
        _logger = logger;
        _sftpsOptions = sftpsOptions.CurrentValue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var sftpOption in _sftpsOptions)
            {
                var authMethod = new PasswordAuthenticationMethod(sftpOption.Username, sftpOption.Password);
                var connectionInfo = new ConnectionInfo(sftpOption.Host, sftpOption.Port, sftpOption.Username, authMethod);
                
                using var sshClient = new SftpClient(connectionInfo);
                sshClient.Connect();

                var files = await sshClient.ListDirectoryAsync("/");
                foreach (var file in files)
                {
                    _logger.LogInformation("[{Tenant}] {FileName}", sftpOption.Name, file.FullName);
                }
            }
            
            await Task.Delay(Interval, stoppingToken);
        }
    }
}