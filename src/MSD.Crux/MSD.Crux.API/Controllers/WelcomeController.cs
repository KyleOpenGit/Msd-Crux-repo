using Microsoft.AspNetCore.Mvc;

namespace MSD.Crux.API.Controllers;

[ApiController]
[Route("api")]
public class WelcomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Welcome to MESCADAS Crux' API");
    }
}
