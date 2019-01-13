using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Api.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FrameDemo.Api.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public class Login
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Login login)
        {
            var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"]));
            if (login.Name == "jack" && login.Password == "rose")
            {
                var result = new
                {
                    token = GenerateToken(serverSecret),
                    token_type = "Bearer"
                };

                return Ok(result);
            }

            return BadRequest(new BadRequestMessage());
        }

        private string GenerateToken(SecurityKey key)
        {
            var now = DateTime.Now;
            var issuer = Configuration["JWT:Issuer"];
            var audience = Configuration["JWT:Audience"];
            var identity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "jack"),
                    new Claim(ClaimTypes.Role, "admin")
                }
            );
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateJwtSecurityToken(
                issuer,
                audience,
                identity,
                now,
                DateTime.Now.AddMinutes(2),
                now,
                signingCredentials
            );
            var jwtToken = handler.WriteToken(token);
            return jwtToken;
        }
    }
}
