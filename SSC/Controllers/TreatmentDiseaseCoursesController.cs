using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
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

        public TreatmentDiseaseCoursesController(IConfiguration config, ITreatmentDiseaseCoursesRepository treatmentDiseaseCoursesRepository)
        {
            _config = config;
            this.treatmentDiseaseCoursesRepository = treatmentDiseaseCoursesRepository;
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
                return Ok(result.Select(x => new
                {
                    x.Date,
                    x.Description,
                    DiseaseCourse = x.DiseaseCourse.Name
                })); ;
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
