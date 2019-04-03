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
    /**
        * @api {get} /authentication 获取所有authentication
        * @apiName GetSamples
        * @apiGroup Authentication
        *
        * @apiParam {name} parameters Users unique ID.
        * @apiParam {Int} PageIndex Firstname of the User.
        * @apiParam {Int} PageSize  Lastname of the User.
        * @apiParam {String} OrderBy  Lastname of the User.
        * @apiParam {String} Fields  Lastname of the User.
        *
        * @apiSuccess {Int} PageIndex Firstname of the User.
        * @apiSuccess {Int} PageSize  Lastname of the User.
        * @apiSuccess {String} OrderBy  Lastname of the User.
        * @apiSuccess {String} Fields  Lastname of the User.
        *
        * @apiSuccessExample Success-Response:
        *     HTTP/1.1 200 OK
        *     {
        *       "firstname": "John",
        *       "lastname": "Doe"
        *     }
        *
        * @apiUse Errors
        *
        */
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
                DateTime.Now.AddMinutes(200),
                now,
                signingCredentials
            );
            var jwtToken = handler.WriteToken(token);
            return jwtToken;
        }
    }
}
