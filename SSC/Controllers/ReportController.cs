using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;

namespace SSC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : CommonController
    {
        private readonly ITestRepository testRepository;
        private readonly ITreatmentDiseaseCoursesRepository treatmentDiseaseCoursesRepository;
        private readonly ITreatmentRepository treatmentRepository;
        public ReportController(ITestRepository testRepository, ITreatmentDiseaseCoursesRepository treatmentDiseaseCoursesRepository, ITreatmentRepository treatmentRepository)
        {
            this.testRepository = testRepository;
            this.treatmentDiseaseCoursesRepository = treatmentDiseaseCoursesRepository;
            this.treatmentRepository = treatmentRepository;
        }

        [HttpGet("tests/{category}/{provinceId}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetTestsReport(string category, Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            var tests = await testRepository.GetTests(provinceId, dateFrom, dateTo);
            if(tests.Count() == 0)
            {
                return NotFound();
            }

            var dict = new Dictionary<string, Func<Test, object>>
            {
                {"type", x => x.TestType.Name },
                {"result", x => x.Result }
            };
            if(!dict.ContainsKey(category))
            {
                return BadRequest();
            }

            var result = tests
                .GroupBy(dict[category])
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / tests.Count)*100 });
            return Ok(result);
        }

        [HttpGet("diseaseCourses/{provinceId}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetTestsReport(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            var diseaseCourses = await treatmentDiseaseCoursesRepository.GetTreatmentDiseaseCourses(provinceId, dateFrom, dateTo);
            if (diseaseCourses.Count() == 0)
            {
                return NotFound();
            }

            var result = diseaseCourses
                .GroupBy(x => x.DiseaseCourse.Name)
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / diseaseCourses.Count) * 100 });
            return Ok(result);
        }

        [HttpGet("treatments/{provinceId}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetTreatment(Guid provinceId, DateTime dateFrom, DateTime dateTo)
        {
            var treatments = await treatmentRepository.GetTreatments(provinceId, dateFrom, dateTo);
            if (treatments.Count() == 0)
            {
                return NotFound();
            }

            var result = treatments
                .GroupBy(x => x.TreatmentStatus.Name)
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / treatments.Count) * 100 });
            return Ok(result);
        }
    }
}
