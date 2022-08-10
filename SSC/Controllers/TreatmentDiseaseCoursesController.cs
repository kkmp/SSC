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
    }
}
