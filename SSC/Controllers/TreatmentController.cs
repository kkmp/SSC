using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO.Treatment;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentController : CommonController
    {
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IMapper mapper;

        public TreatmentController(ITreatmentRepository treatmentRepository, IMapper mapper)
        {
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addTreatment")]
        public async Task<IActionResult> AddTreatment(TreatmentCreateDTO treatment)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await treatmentRepository.AddTreatment(treatment, issuerId);
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

        [HttpGet("showTreatments/{patientId}")]
        public async Task<IActionResult> ShowTreatments(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentRepository.ShowTreatments(patientId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(mapper.Map<List<TreatmentOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("showTreatmentDetails/{treatmentId}")]
        public async Task<IActionResult> ShowTreatmentDetails(Guid treatmentId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentRepository.ShowTreatmentDetails(treatmentId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(mapper.Map<TreatmentGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

            [HttpPut("editTreatment")]
            public async Task<IActionResult> EditTreatment(TreatmentUpdateDTO treatment)
            {
                if (ModelState.IsValid)
                {
                    var issuerId = GetUserId();
                    var result = await treatmentRepository.EditTreatment(treatment, issuerId);
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
    }
}
