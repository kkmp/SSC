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
            if (user == null)
            {
                return NotFound(new { errors = new { Message = new string[] { "Nie znaleziono adresu email użytkownika" } } });
            }
            var result = await changePasswordRepository.AddCode(user.Id);
            if (!result.Success)
            {
                return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
            }

            await mailService.SendEmailAsync(new MailRequest(model.Email, "Zmiana hasła", "Twój link do zmiany hasła: http://localhost:3000/changePassword/useCode/" + result.Data.Code));

            return Ok(new { message = "Wiadomość została wysłana na wskazny adres email" });
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(PasswordUpdateDTO password)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await changePasswordRepository.ChangePassword(password.OldPassword, password.NewPassword, issuerId);

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

        [AllowAnonymous]
        [HttpPost("byCode")]
        public async Task<IActionResult> ChangeCode(ChangePasswordUpdateDTO password)
        {
            if (ModelState.IsValid)
            {
                var result = await changePasswordRepository.ChangeCode(password.Password, password.Code);

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
    }
}
