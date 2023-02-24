using CleanArchMvc.API.Models;
using CleanArchMvc.Domain.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchMvc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticate _authentication;
        private readonly IConfiguration _configuration;

        public TokenController(IAuthenticate authentication, IConfiguration configuration)
        {
            _authentication = authentication ??
                throw new ArgumentNullException(nameof(authentication));
            _configuration = configuration;
        }

        [HttpPost("CreateUser")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateUser([FromBody] LoginModel userInfo)
        {
            var result = await _authentication.RegisterUser(userInfo.Email, userInfo.Password);

            if (result)
                return Ok($"User {userInfo.Email}, was create successfully");
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt");
                return BadRequest(ModelState);
            }
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
        {
            var result = await _authentication.Authenticate(userInfo.Email, userInfo.Password);

            if (result)
                return GenerateToken(userInfo);
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt");
                return BadRequest(ModelState);
            }
        }

        private UserToken GenerateToken(LoginModel userInfo)
        {
            //Declaração do usuário
            var claims = new[]
            {
                new Claim("email", userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Gerar chave privada para assinar o token
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            //Gerar assinatura digital
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            //Definir tempo de expiração
            var expiration = DateTime.UtcNow.AddMinutes(10);

            //Gerar o token
            JwtSecurityToken token = new JwtSecurityToken(
                //emissor
                issuer: _configuration["Jwt:Issuer"],
                //audiencia
                audience: _configuration["Jwt:Audience"],
                //claims
                claims: claims,
                //data expiração
                expires: expiration,
                //assinatura digital
                signingCredentials: credentials
                );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
