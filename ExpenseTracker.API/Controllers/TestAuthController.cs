using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var email =
            User.FindFirstValue(ClaimTypes.Email);

        return Ok(new
        {
            message = "Authorization successful.",
            userId,
            email
        });
    }


}