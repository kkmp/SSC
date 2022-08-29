using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
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
        private readonly IMapper mapper;

        public MedicalHistoryController(IConfiguration config, IMedicalHistoryRepository medicalHistoryRepository, IMapper mapper)
        {
            _config = config;
            this.medicalHistoryRepository = medicalHistoryRepository;
            this.mapper = mapper;
        }

        [HttpPost("addMedicalHistory")]
        public async Task<IActionResult> AddMedicalHistory(MedicalHistoryViewModel medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await medicalHistoryRepository.AddMedicalHistory(medicalHistory, issuerId);
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
        public async Task<IActionResult> EditMedicalHistory(MedicalHistoryEditViewModel medicalHistory)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await medicalHistoryRepository.EditMedicalHistory(medicalHistory, issuerId);
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

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(mapper.Map<List<MedicalHistoryDTO>>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
