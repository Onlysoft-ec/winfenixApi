using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using winfenixApi.Core.Interfaces;
using winfenixApi.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IAuthService _authService;

    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request)
    {
        var token = await _authService.GenerateJwtTokenAsync(request.Username, request.Password);
        if (token == null)
        {
            return Unauthorized();
        }
        return Ok(new { Token = token });
    }

    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var isValid = _authService.ValidateTokenAsync(token).Result;
        if (!isValid)
        {
            return Unauthorized();
        }
        return Ok();
    }
}
