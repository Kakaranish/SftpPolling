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
    
    [HttpPost("cpu/{cpuPercentage}")]
    public async Task<IActionResult> LoadCpu([FromRoute] int cpuPercentage)
    {
        var executionId = Guid.NewGuid();
        
        _logger.LogInformation("[{ExecutionId}] Starting CPU {CpuPercentage}% load", 
            executionId, cpuPercentage);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
        
        ConsumeCpu(executionId, cpuPercentage, cancellationTokenSource.Token);
        
        return Ok();
    }

    private async Task ConsumeCpu(Guid executionId, int cpuPercentage, CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            if (cpuPercentage < 0 || cpuPercentage > 100)
                throw new ArgumentException("percentage");

            var watch = new Stopwatch();
            watch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                // Make the loop go on for "percentage" milliseconds then sleep the 
                // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                if (watch.ElapsedMilliseconds > cpuPercentage)
                {
                    Thread.Sleep(100 - cpuPercentage);
                    watch.Reset();
                    watch.Start();
                }
            }
            
            _logger.LogInformation("[{ExecutionId}] CPU {CpuPercentage}% load finished", 
                executionId, cpuPercentage);
            
        }, cancellationToken);
    }
}