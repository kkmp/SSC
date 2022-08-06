using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.Tools;

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

        private byte[] CreateExcel(List<Test> tests)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add("results");
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                string[] columns = { "TestDate", "Result", "Place" };
                for(int i = 1; i <= columns.Length; i++)
                {
                    ws.Cells[1, i].Value = columns[i - 1];
                }
                for(int i = 0; i < tests.Count; i++)
                {
                    ws.Cells[i + 2, 1].Value = tests[i].TestDate;
                    ws.Cells[i + 2, 2].Value = tests[i].Result;
                    ws.Cells[i + 2, 3].Value = tests[i].Place.Name;
                }
                return package.GetAsByteArray();
            }    
        }

        [HttpGet("tests/{category}/{provinceId}/{dateFrom}/{dateTo}/")]
        [HttpGet("tests/{category}/{provinceId}/{dateFrom}/{dateTo}/{type}")]
        public async Task<IActionResult> GetTestsReport(string category, Guid provinceId, DateTime dateFrom, DateTime dateTo, string? type)
        {
            var tests = await testRepository.GetTests(provinceId, dateFrom, dateTo);
            if(type == "xlsx")
            {
                return File(CreateExcel(tests), "application/xlsx", "results.xlsx");
            }
            else if(type == "csv")
            {
                return File(System.Text.Encoding.UTF8.GetBytes(ICSV.CreateCSV(tests.Cast<ICSV>().ToList())), "application/csv", "results.csv");
            }

            if (tests.Count() == 0)
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
