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
    
    [HttpPost("cpu/{DurationInSeconds}")]
    public async Task<IActionResult> LoadCpu([FromRoute] int durationInSeconds)
    {
        var executionId = Guid.NewGuid();
        
        _logger.LogInformation("[{ExecutionId}] Starting CPU load", 
            executionId);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(durationInSeconds));
        
        ConsumeCpu(executionId, cancellationTokenSource.Token);
        
        return Ok();
    }

    private async Task ConsumeCpu(Guid executionId, CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var random = new Random();
                var x = random.NextDouble() / random.NextDouble() % 10 * random.NextDouble();
            }

            _logger.LogInformation("[{ExecutionId}] CPU load finished", 
                executionId);
            
        }, cancellationToken);
    }
}