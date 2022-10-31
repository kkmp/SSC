using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SSC.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : CommonController
    {
        private IConfiguration _config;
        private readonly IUnitOfWork unitOfWork;

        public LoginController(IConfiguration _config, IUnitOfWork unitOfWork)
        {
            this._config = _config;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO login)
        {
            IActionResult response;

            var result = await unitOfWork.UserRepository.AuthenticateUser(login.Email, login.Password);

            if (result.Success)
            {
                var role = await unitOfWork.RoleRepository.GetRole(result.Data.RoleId.Value);
                if (!role.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                result.Data.Role = role.Data;

                var tokenString = GenerateJSONWebToken(result.Data);
                response = Ok(new { token = tokenString });
            }
            else
            {
                response = BadRequest(new { errors = new { Message = new string[] { result.Message } } });
            }
            return response;
        }
         private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, userInfo.Name),
            new Claim(ClaimTypes.Surname, userInfo.Surname),
            new Claim(ClaimTypes.Role, userInfo.Role.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
