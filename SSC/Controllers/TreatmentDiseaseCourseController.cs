using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.TreatmentDiseaseCourse;

namespace SSC.Controllers
{
    [Authorize(Roles = "Lekarz,Administrator")]
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentDiseaseCourseController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public TreatmentDiseaseCourseController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("addTreatmentDiseaseCourse")]
        public async Task<IActionResult> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.TreatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourse, issuerId);
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

        [HttpGet("showTreatmentDiseaseCourses/{patientId}")]
        public async Task<IActionResult> ShowTreatmentDiseaseCourses(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourses(patientId);
                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(unitOfWork.Mapper.Map<List<TreatmentDiseaseCourseOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("showTreatmentDiseaseCourseDetails/{treatmentDiseaseCourseId}")]
        public async Task<IActionResult> ShowTreatmentDiseaseCourseDetails(Guid treatmentDiseaseCourseId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.TreatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourseDetails(treatmentDiseaseCourseId);
                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(unitOfWork.Mapper.Map<TreatmentDiseaseCourseGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpPut("editTreatmentDiseaseCourse")]
        public async Task<IActionResult> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.TreatmentDiseaseCourseRepository.EditTreatmentDiseaseCourse(treatmentDiseaseCourse, issuerId);
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
