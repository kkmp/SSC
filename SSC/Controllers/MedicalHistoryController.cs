using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.MedicalHistory;

namespace SSC.Controllers
{
    [Authorize(Roles = "Lekarz,Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalHistoryController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public MedicalHistoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("addMedicalHistory")]
        public async Task<IActionResult> AddMedicalHistory(MedicalHistoryCreateDTO medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.MedicalHistoryRepository.AddMedicalHistory(medicalHistory, issuerId);
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

        [HttpPut("editMedicalHistory")]
        public async Task<IActionResult> EditMedicalHistory(MedicalHistoryUpdateDTO medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.MedicalHistoryRepository.EditMedicalHistory(medicalHistory, issuerId);
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

        [HttpGet("showMedicalHistories/{patientId}")]
        public async Task<IActionResult> ShowMedicalHistories(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.MedicalHistoryRepository.ShowMedicalHistories(patientId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<List<MedicalHistoryOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }


        [HttpGet("showMedicalHistoryDetails/{medicalHistoryId}")]
        public async Task<IActionResult> ShowMedicalHistoryDetails(Guid medicalHistoryId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.MedicalHistoryRepository.ShowMedicalHistoryDetails(medicalHistoryId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<MedicalHistoryGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }
    }
}
