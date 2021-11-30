using JWTRestService.Data;
using JWTRestService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTRestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;

        public TokenController(ILogger<TokenController> logger)
        {
            _logger = logger;
        }

        //[HttpGet("authorization")]
        //[Authorize(Roles = "Administrators")]
        [HttpGet]
        [Route("Sram/")]
        public ActionResult Sram()
        {
            return Ok();
        }
            

        [HttpPost]
        [Route("Token/(login)/(password)")]    ///{id} - requared param
        public ActionResult Token(string login, string password)
        {
            //return Ok(new { DateTime.Now, id, login });
            var identity = GetIdentity(login, password);//"admin@gmail.com", "12345"
            if (identity == null)
            {
                //Console.WriteLine("Invalid username or password.");
                return BadRequest("Invalid username or password");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthConstants.ISSUER,
                    audience: AuthConstants.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthConstants.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthConstants.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Ok(new { encodedJwt });
            //return Content( response.access_token );
        }

        private static ClaimsIdentity GetIdentity(string username, string password)
        {
            Person person = AbstractDB.people.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, person.Login),
                    new Claim(JwtRegisteredClaimNames.Sub, person.Id.ToString()),
                    //new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
