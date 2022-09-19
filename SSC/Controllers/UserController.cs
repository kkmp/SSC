using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.DTO.User;
using SSC.Models;
using SSC.Services;
using SSC.Tools;
using System.ComponentModel.DataAnnotations;
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
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(UserCreateDTO user)
        {
            if (ModelState.IsValid)
            {
                var result = await userRepository.AddUser(user);
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
        [HttpPut("changeActivity/{option}")]
        public async Task<IActionResult> ChangeActivity(string option, IdCreateDTO userId)
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
                        return BadRequest(new { errors = new { Message = new string[] { "Incorrect option" } } });
                }

                var issuer = GetUserId();
                var result = await userRepository.ChangeActivity(userId.Id, issuer, activation);

                if (result.Success)
                {
                    return Ok(new { message = result.Message });

                }
                else
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("userDetails/{userid}")]
        public async Task<IActionResult> UserDetails(Guid userid)
        {
            if (ModelState.IsValid)
            {
                var result = await userRepository.UserDetails(userid);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });

                }

                return Ok(mapper.Map<UserDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });

        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("editUser")]
        public async Task<IActionResult> EditUser(UserUpdateDTO user)
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

        [Authorize(Roles = "Administrator")]
        [HttpGet("filterUsers/{pageNr}/{option}/{orderType}")]
        [HttpGet("filterUsers/{pageNr}/{option}/{orderType}/{searchName}")]
        public async Task<IActionResult> FilterUsers([Range(1, 100000000)] int pageNr, string option, string orderType, string? searchName)
        {
            IEnumerable<User> result = await userRepository.GetUsers();

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
                    return BadRequest(new { errors = new { Message = new string[] { "Incorrect filter option" } } });

            }

            if (searchName != null)
            {
                searchName = searchName.ToLower();
                result = result
                    .Where(x => x.Name.ToLower().Contains(searchName)
                    || x.Surname.ToLower().Contains(searchName)
                    || x.Email.ToLower().Contains(searchName)
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName));
            }

            result = result.GetPage(pageNr, 3).ToList();

            return Ok(mapper.Map<List<UserOverallDTO>>(result));
        }
    }
}
