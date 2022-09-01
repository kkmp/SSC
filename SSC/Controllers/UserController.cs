using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : CommonController
    {
        private IConfiguration _config;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;

        public UserController(IConfiguration config, IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _config = config;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await userRepository.AddUser(user);
                var msg = new { errors = new { Email = new string[] { result.Message } } };
                if (result.Success)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginViewModel login)
        {
            IActionResult response;

            var result = await userRepository.AuthenticateUser(login.Email, login.Password);
            
            if (result.Success)
            {
                var role = await roleRepository.GetRole(result.Data.RoleId.Value);
                if (!role.Success)
                {
                    return BadRequest(new { message = role.Message });
                }

                result.Data.Role = role.Data;

                var tokenString = GenerateJSONWebToken(result.Data);
                response = Ok(new { token = tokenString });
            }
            else
            {
                var msg = new { errors = new { Email = new string[] { result.Message } } };
                response = BadRequest(msg);
            }
            return response;
        }


        [Authorize(Roles = "Administrator")]
        [HttpPut("changeActivity/{option}")]
        public async Task<IActionResult> ChangeActivity(string option, IdViewModel userId)
        {
            if (ModelState.IsValid)
            {
                bool activation = false;
                switch (option)
                {
                    case "activate":
                        activation = true;
                        break;
                    case "deactivate":
                        activation = false;
                        break;
                    default:
                        return BadRequest(new { message = "Incorrect option" });
                }

                var issuer = GetUserId();
                var result = await userRepository.ChangeActivity(userId.Id, issuer, activation);
                    var msg = new { message = result.Message };

                    if (result.Success)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        return BadRequest(msg);
                    }
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("userDetails")]
        public async Task<IActionResult> UserDetails(IdViewModel userid)
        {
            if (ModelState.IsValid)
            {
                var result = await userRepository.UserDetails(userid.Id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(mapper.Map<UserDTO>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("editUser")]
        public async Task<IActionResult> EditUser(UserEditViewModel user)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await userRepository.EditUser(user, issuerId);

                var msg = new { message = result.Message };
                if (result.Success)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }
            return BadRequest(new { message = "Invalid data" });
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString()),
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

        [Authorize(Roles = "Administrator")]
        [HttpGet("filterUsers/{option}/{orderType}")]
        [HttpGet("filterUsers/{option}/{orderType}/{searchName}")]
        public async Task<IActionResult> FilterUsers(string option, string orderType, string? searchName)
        {
            List<User> result = await userRepository.GetUsers();

            switch (option)
            {
                case "surname":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.Surname) : result.OrderBy(x => x.Surname)).ToList();
                    break;
                case "email":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.Email) : result.OrderBy(x => x.Email)).ToList();
                    break;
                case "active":
                    result = (orderType == "descending" ? result.OrderByDescending(x => x.IsActive) : result.OrderBy(x => x.IsActive)).ToList();
                    break;
                default:
                    return BadRequest(new { message = "Incorrect filter option" });
            }

            if (searchName != null)
            {
                searchName = searchName.ToLower();
                result = result
                    .Where(x => x.Name.ToLower().Contains(searchName)
                    || x.Surname.ToLower().Contains(searchName)
                    || x.Email.ToLower().Contains(searchName)
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName))
                    .ToList();
            }

            return Ok(mapper.Map<List<UserOverallDTO>>(result));
        }
    }
}
