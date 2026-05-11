using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers; 
[ApiController]
[Route("health")]
public class HealthController(IHostEnvironment env) : ControllerBase {
    private readonly IHostEnvironment _env = env;

    [HttpGet]
    public IActionResult Get() {
        return Ok(new { 
            status = "Healthy",
            environment = _env.EnvironmentName,
            timestemp = DateTimeOffset.UtcNow
        });
    }
}
