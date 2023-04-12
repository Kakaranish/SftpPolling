using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SftpPolling.Controllers;

public class DebugInfo
{
    public string? Environment { get; set; }
    public string? EnvironmentIdentifier { get; set; }
}

[ApiController]
[Route("api")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;
    private readonly IConfiguration _configuration;

    public MainController(ILogger<MainController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("debug")]
    public DebugInfo GetDebug()
    {
        return new DebugInfo
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            EnvironmentIdentifier = _configuration.GetValue<string>("EnvironmentIdentifier")
        };
    }
    
    [HttpGet("hello")]
    public async Task<IActionResult> GetHello()
    {
        _logger.LogInformation("[{Time}] Saying hello", DateTime.UtcNow.ToString("o"));

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));

        await ConsumeCpu(80, cancellationTokenSource.Token);
        
        return Ok();
    }

    private static Task ConsumeCpu(int percentage, CancellationToken cancellationToken)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("percentage");
        
        var watch = new Stopwatch();
        watch.Start();            
        while (!cancellationToken.IsCancellationRequested)
        {
            // Make the loop go on for "percentage" milliseconds then sleep the 
            // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
            if (watch.ElapsedMilliseconds > percentage)
            {
                Thread.Sleep(100 - percentage);
                watch.Reset();
                watch.Start();
            }
        }

        return Task.CompletedTask;
    }
}