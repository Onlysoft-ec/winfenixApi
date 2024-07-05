using Microsoft.AspNetCore.Mvc;
using winfenixApi.Application.Interfaces;
using winfenixApi.Core.Entities;

namespace winfenixApi.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] InputLoginDTO input)
        {
            var authResponse = _loginService.Autenticador(input);
            if (authResponse.Succeeded)
            {
                var tokenResponse = _loginService.GeneraToken(input);
                return tokenResponse.Succeeded ? Ok(tokenResponse) : BadRequest(tokenResponse.Message);
            }
            return BadRequest(authResponse.Message);
        }
    }
}
