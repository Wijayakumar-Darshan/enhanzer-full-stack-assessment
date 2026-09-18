using Microsoft.AspNetCore.Mvc;
using PurchaseBillApi.DTOs;
using PurchaseBillApi.Services;

namespace PurchaseBillApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>POST api/auth/login - Task 1: authenticate against the external POS API.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new LoginResponseDto { Success = false, Message = "Invalid request." });
        }

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}
