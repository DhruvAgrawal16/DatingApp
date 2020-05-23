using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.ApI.Dtos;
using DatingApp.ApI.Models;
using dotnet_rpg.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.ApI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this.config = config;
            this.repo = repo;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegisterDto user)
        {
            user.Username = user.Username.ToLower();
            if (await repo.UserExist(user.Username) == true)
            {
                return BadRequest("User already exist");
            }

            User userToCreate = new User
            {
                Username = user.Username
            };

            User createdUser = await repo.Register(userToCreate, user.Password);
            return StatusCode(201);

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {

            var userFromRepo = await repo.Login(user.Username.ToLower(), user.Password);
            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                  new Claim(ClaimTypes.Name,userFromRepo.Username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSetting:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
                
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token = tokenHandler.WriteToken(token)
            });        }
    }
}