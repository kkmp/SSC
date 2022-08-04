using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class UserController : CommonController
    {
        private IConfiguration _config;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;

        public UserController(IConfiguration config, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _config = config;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel user)
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
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel login)
        {
            IActionResult response;
            var result = await userRepository.AuthenticateUser(login.Email, login.Password);
            
            if (result.Success)
            {
                result.Data.Role = await roleRepository.GetRole(result.Data.RoleId.Value);
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

        //Funkcja testowa!!!!!
        [Authorize(Roles = "Administrator")]
        [HttpGet("showUsers")]
        public async Task<IActionResult> ShowUsers()
        {
                var result = await userRepository.GetUsers();
                return Ok(result);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("deactivateUser")]
        public async Task<IActionResult> DeactivateUser([FromBody] UserEmailViewModel useremail)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await userRepository.DeactivateUser(useremail.Email, id);
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
        [HttpPut("activateUser")]
        public async Task<IActionResult> ActivateUser([FromBody] UserEmailViewModel useremail)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await userRepository.ActivateUser(useremail.Email, id);
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
        public async Task<IActionResult> UserDetails([FromBody] IdViewModel userid)
        {
            if (ModelState.IsValid)
            {
                var result = await userRepository.UserDetails(userid.Id);
                return Ok(new
                {
                    result.Name,
                    result.Surname,
                    result.Email,
                    Date = result.Date?.ToString(),
                    result.IsActive,
                    Role = result.Role.Name,
                    result.PhoneNumber,
                });
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("editUser")]
        public async Task<IActionResult> EditUser([FromBody] UserEditViewModel user)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await userRepository.EditUser(user, id);

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
        public async Task<IActionResult> FilterPatients(string option, string orderType, string? searchName)
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
            return Ok(result.Select(x => new
            {
                x.Id,
                x.Name,
                x.Surname,
                x.Email,
                x.PhoneNumber,
                x.IsActive,
                Role = x.Role.Name
            }));
        }
    }
}
