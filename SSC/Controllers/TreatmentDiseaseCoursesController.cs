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
    public class TreatmentDiseaseCoursesController : CommonController
    {
        private IConfiguration _config;
        private readonly ITreatmentDiseaseCoursesRepository treatmentDiseaseCoursesRepository;
        private readonly IMapper mapper;

        public TreatmentDiseaseCoursesController(IConfiguration config, ITreatmentDiseaseCoursesRepository treatmentDiseaseCoursesRepository, IMapper mapper)
        {
            _config = config;
            this.treatmentDiseaseCoursesRepository = treatmentDiseaseCoursesRepository;
            this.mapper = mapper;
        }

        [HttpPost("addTreatmentDiseaseCourse")]
        public async Task<IActionResult> AddTreatmentDiseaseCourse(TreatmentDiseaseCoursesViewModel treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await treatmentDiseaseCoursesRepository.AddTreatmentDiseaseCourse(treatmentDiseaseCourse, id);
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
                var result = await treatmentDiseaseCoursesRepository.ShowTreatmentDiseaseCourses(patientId.Id);
                //select się może popsuć gdy null
                return Ok(mapper.Map<List<TreatmentDiseaseCourseDTO>>(result));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpPut("editTreatmentDiseaseCourse")]
        public async Task<IActionResult> EditTreatmentDiseaseCourse(TreatmentDiseaseCourseEditViewModel treatmentDiseaseCourse)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await treatmentDiseaseCoursesRepository.EditTreatmentDiseaseCourses(treatmentDiseaseCourse, id);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
