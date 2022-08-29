using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.Models;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentDiseaseCourseController : CommonController
    {
        private IConfiguration _config;
        private readonly ITreatmentDiseaseCourseRepository treatmentDiseaseCourseRepository;
        private readonly IMapper mapper;

        public TreatmentDiseaseCourseController(IConfiguration config, ITreatmentDiseaseCourseRepository treatmentDiseaseCourseRepository, IMapper mapper)
        {
            _config = config;
            this.treatmentDiseaseCourseRepository = treatmentDiseaseCourseRepository;
            this.mapper = mapper;
        }

        [HttpPost("addTreatmentDiseaseCourse")]
        public async Task<IActionResult> AddTreatmentDiseaseCourse(TreatmentDiseaseCourseViewModel treatmentDiseaseCourse)
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
        public async Task<IActionResult> ShowTreatmentDiseaseCourses(IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentDiseaseCourseRepository.ShowTreatmentDiseaseCourses(patientId.Id);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(mapper.Map<List<TreatmentDiseaseCourseDTO>>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpPut("editTreatmentDiseaseCourse")]
        public async Task<IActionResult> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseEditViewModel treatmentDiseaseCourse)
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
