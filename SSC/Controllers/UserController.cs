using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.DTO.User;
using SSC.Tools;
using System.ComponentModel.DataAnnotations;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser(UserCreateDTO user)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.UserRepository.AddUser(user);
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
        [HttpPut("changeActivity/{option}/{userId}")]
        public async Task<IActionResult> ChangeActivity(string option, Guid userId)
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
                        return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja" } } });
                }

                var issuer = GetUserId();
                var result = await unitOfWork.UserRepository.ChangeActivity(userId, issuer, activation);

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
                var result = await unitOfWork.UserRepository.UserDetails(userid);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<UserDTO>(result.Data));
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
                var result = await unitOfWork.UserRepository.EditUser(user, issuerId);

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
        [HttpGet("filterUsers/{pageNr}/{option}/{orderType}")]
        [HttpGet("filterUsers/{pageNr}/{option}/{orderType}/{searchName}")]
        public async Task<IActionResult> FilterUsers([Range(1, 100000000)] int pageNr, string option, string orderType, string? searchName)
        {
            IEnumerable<User> result = await unitOfWork.UserRepository.GetUsers();

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
                    return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja filtrowania" } } });

            }

            if (searchName != null)
            {
                searchName = NormalizeWhiteSpaceExtension.NormalizeWhiteSpaceForLoop(searchName.ToLower().Trim());
                result = result
                    .Where(x => x.Name.ToLower().Contains(searchName)
                    || x.Surname.ToLower().Contains(searchName)
                    || x.Email.ToLower().Contains(searchName)
                    || (x.Name + " " + x.Surname).ToLower().Contains(searchName)
                    || (x.Surname + " " + x.Name).ToLower().Contains(searchName));
            }

            result = result.GetPage(pageNr, 3).ToList();

            return Ok(unitOfWork.Mapper.Map<List<UserOverallDTO>>(result));
        }
    }
}
