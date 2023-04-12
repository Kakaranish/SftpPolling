using Microsoft.AspNetCore.Mvc;

namespace SftpPolling.Controllers;

[ApiController]
[Route("api")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;

    public MainController(ILogger<MainController> logger)
    {
        _logger = logger;
    }

    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        _logger.LogInformation("[{Time}] Saying hello", DateTime.UtcNow.ToString("o"));
        return Ok();
    }
}