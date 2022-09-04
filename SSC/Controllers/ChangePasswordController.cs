using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.Models;
using SSC.Services;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChangePasswordController : CommonController
    {
        private readonly IUserRepository userRepository;
        private readonly IChangePasswordRepository changePasswordRepository;
        private readonly IMailService mailService;

        public ChangePasswordController(IUserRepository userRepository, IChangePasswordRepository changePasswordRepository, IMailService mailService)
        {
            this.userRepository = userRepository;
            this.changePasswordRepository = changePasswordRepository;
            this.mailService = mailService;
        }

        [AllowAnonymous]
        [HttpPost("getCode")]
        public async Task<IActionResult> GetCode(EmailCreateDTO model)
        {
            var user = await userRepository.GetUserByEmail(model.Email);
            if(user == null)
            {
                return NotFound("User's email not found");
            }
            var result = await changePasswordRepository.AddCode(user.Id);
            if(!result.Success)
            {
                return BadRequest(result.Message);
            }

            await mailService.SendEmailAsync(new MailRequest(model.Email, "Zmiana hasła", "Twój link do zmiany hasła: http://localhost:7090/api/ChangePassword/code/" + result.Data.Code));

            return Ok("Email has been sent");
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(PasswordUpdateDTO password)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await changePasswordRepository.ChangePassword(password.Password, issuerId);

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
    }
}
