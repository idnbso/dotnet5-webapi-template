using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IJWTAuthenticationManager _jwtAuthenticationManager;

        public AuthController(ILogger<AuthController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _logger = logger;
            _jwtAuthenticationManager = jWTAuthenticationManager;
        }

        [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserAuth userAuth)
        {
            var token = _jwtAuthenticationManager.Authenticate(userAuth.UserName, userAuth.Password);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            var token = _jwtAuthenticationManager.Authenticate();

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}