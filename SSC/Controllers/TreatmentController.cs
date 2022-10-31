using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.Treatment;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public TreatmentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("addTreatment")]
        public async Task<IActionResult> AddTreatment(TreatmentCreateDTO treatment)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.TreatmentRepository.AddTreatment(treatment, issuerId);
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
                var result = await unitOfWork.TreatmentRepository.ShowTreatments(patientId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<List<TreatmentOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("showTreatmentDetails/{treatmentId}")]
        public async Task<IActionResult> ShowTreatmentDetails(Guid treatmentId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.TreatmentRepository.ShowTreatmentDetails(treatmentId);

                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }

                return Ok(unitOfWork.Mapper.Map<TreatmentGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

            [HttpPut("editTreatment")]
            public async Task<IActionResult> EditTreatment(TreatmentUpdateDTO treatment)
            {
                if (ModelState.IsValid)
                {
                    var issuerId = GetUserId();
                    var result = await unitOfWork.TreatmentRepository.EditTreatment(treatment, issuerId);
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
