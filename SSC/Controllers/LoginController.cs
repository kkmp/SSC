using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO.User;
using SSC.Models;
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
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;

        public LoginController(IConfiguration _config, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this._config = _config;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO login)
        {
            IActionResult response;

            var result = await userRepository.AuthenticateUser(login.Email, login.Password);

            if (result.Success)
            {
                var role = await roleRepository.GetRole(result.Data.RoleId.Value);
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
