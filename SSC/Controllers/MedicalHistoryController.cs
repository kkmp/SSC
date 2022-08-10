using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.Models;

namespace SSC.Controllers
{
    [Authorize(Roles = "Lekarz,Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalHistoryController : CommonController
    {
        private IConfiguration _config;
        private readonly IMedicalHistoryRepository medicalHistoryRepository;

        public MedicalHistoryController(IConfiguration config, IMedicalHistoryRepository medicalHistoryRepository)
        {
            _config = config;
            this.medicalHistoryRepository = medicalHistoryRepository;
        }

        [HttpPost("addMedicalHistory")]
        public async Task<IActionResult> AddMedicalHistory(MedicalHistoryViewModel medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await medicalHistoryRepository.AddMedicalHistory(medicalHistory, id);
                var msg = new { errors = new { Message = new string[] { result.Message } } };
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

        [HttpPut("editMedicalHistory")]
        public async Task<IActionResult> EditMedicalHistory(EditMedicalHistoryViewModel medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await medicalHistoryRepository.EditMedicalHistory(medicalHistory, id);
                var msg = new { errors = new { Message = new string[] { result.Message } } };
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

        [HttpGet("showMedicalHistories")]
        public async Task<IActionResult> ShowMedicalHistories(IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await medicalHistoryRepository.ShowMedicalHistories(patientId.Id);
                //select się może popsuć gdy null
                return Ok(result.Select(x => new
                {
                    x.Date,
                    x.Description,
                    UserName = x.User.Name,
                    UserSurname = x.User.Surname,
                    UserRole = x.User.Role.Name,
                }));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
