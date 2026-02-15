using Microsoft.AspNetCore.Mvc;

namespace GrpcServer.Infrastructure.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    [HttpGet("health/live")]
    public IActionResult Liveness()
    {
        return Ok(new { status = "alive" });
    }

    [HttpGet("health/ready")]
    public IActionResult Readiness()
    {
        return Ok(new { status = "ready" });
    }
}

