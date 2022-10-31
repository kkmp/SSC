using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.Email;
using SSC.DTO.MailRequest;
using SSC.DTO.Password;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChangePasswordController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public ChangePasswordController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPost("getCode")]
        public async Task<IActionResult> GetCode(EmailCreateDTO model)
        {
            var user = await unitOfWork.UserRepository.GetUserByEmail(model.Email);
            if (user == null)
            {
                return NotFound(new { errors = new { Message = new string[] { "Nie znaleziono adresu email użytkownika" } } });
            }
            var result = await unitOfWork.ChangePasswordRepository.AddCode(user.Id);
            if (!result.Success)
            {
                return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
            }

            await unitOfWork.MailService.SendEmailAsync(new MailRequestDTO(model.Email, "Zmiana hasła", "Twój link do zmiany hasła: http://localhost:3000/changePassword/useCode/" + result.Data.Code));

            return Ok(new { message = "Wiadomość została wysłana na wskazny adres email" });
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(PasswordUpdateDTO password)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.ChangePasswordRepository.ChangePassword(password.OldPassword, password.NewPassword, issuerId);

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
                var result = await unitOfWork.ChangePasswordRepository.ChangeCode(password.Password, password.Code);

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
