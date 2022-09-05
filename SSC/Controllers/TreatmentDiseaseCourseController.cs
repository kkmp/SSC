using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.DTO.TreatmentDiseaseCourse;
using SSC.Models;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentDiseaseCourseController : CommonController
    {
        private readonly ITreatmentDiseaseCourseRepository treatmentDiseaseCourseRepository;
        private readonly IMapper mapper;

        public TreatmentDiseaseCourseController(ITreatmentDiseaseCourseRepository treatmentDiseaseCourseRepository, IMapper mapper)
        {
            this.treatmentDiseaseCourseRepository = treatmentDiseaseCourseRepository;
            this.mapper = mapper;
        }

        [HttpPost("addTreatmentDiseaseCourse")]
        public async Task<IActionResult> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseCreateDTO treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await treatmentDiseaseCourseRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourse, issuerId);
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

        [HttpGet("showTreatmentDiseaseCourses")]
        public async Task<IActionResult> ShowTreatmentDiseaseCourses(IdCreateDTO patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourses(patientId.Id);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(mapper.Map<List<TreatmentDiseaseCourseGetDTO>>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpPut("editTreatmentDiseaseCourse")]
        public async Task<IActionResult> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseUpdateDTO treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await treatmentDiseaseCourseRepository.EditTreatmentDiseaseCourse(treatmentDiseaseCourse, issuerId);
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
