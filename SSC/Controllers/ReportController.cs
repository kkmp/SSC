using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.UnitOfWork;
using SSC.Tools;

namespace SSC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Lekarz,Administrator")]
    public class ReportController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("tests/{category}/{provinceId}/{dateFrom}/{dateTo}/")]
        [HttpGet("tests/{category}/{provinceId}/{dateFrom}/{dateTo}/{type}")]
        public async Task<IActionResult> GetTestsReport(string category, Guid provinceId, DateTime dateFrom, DateTime dateTo, string? type)
        {
            var tests = await unitOfWork.TestRepository.GetTests(provinceId, dateFrom, dateTo);

            switch (type)
            {
                case "xlsx":
                    return File(ExcelExtension.CreateExcel(tests, new[] { "OrderNumber", "TestDate", "TestType", "Result", "Place" },
                        x => new object[] {x.OrderNumber, x.TestDate.ToString(), x.TestType.Name, x.Result, x.Place.Name}), "application/xlsx", "results.xlsx");
                case "csv":
                    return File(System.Text.Encoding.UTF8.GetBytes(ICSV.CreateCSV(tests.Cast<ICSV>().ToList())), "application/csv", "results.csv");
                case null:
                    break;
                default:
                    return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja typu pliku" } } });
            }

            if (tests.Count() == 0)
            {
                return BadRequest(new { errors = new { Message = new string[] { "Nie znaleziono danych dla wybranych filtrów!" } } });
            }

            var dict = new Dictionary<string, Func<Test, object>>
            {
                {"type", x => x.TestType.Name },
                {"result", x => x.Result }
            };

            if (!dict.ContainsKey(category))
            {
                return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna kategoria" } } });
            }

            var result = tests
                .GroupBy(dict[category])
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / tests.Count) * 100 });

            return Ok(result);
        }

        [HttpGet("diseaseCourses/{provinceId}/{dateFrom}/{dateTo}")]
        [HttpGet("diseaseCourses/{provinceId}/{dateFrom}/{dateTo}/{type}")]
        public async Task<IActionResult> GetTestsReport(Guid provinceId, DateTime dateFrom, DateTime dateTo, string? type)
        {
            var diseaseCourses = await unitOfWork.TreatmentDiseaseCourseRepository.GetTreatmentDiseaseCourses(provinceId, dateFrom, dateTo);

            switch (type)
            {
                case "xlsx":
                    return File(ExcelExtension.CreateExcel(diseaseCourses, new[] { "Date", "Description", "DiseaseCourse", "DiseaseCourseDescription" },
                        x => new object[] { x.Date.ToString(), x.Description, x.DiseaseCourse.Name, x.DiseaseCourse.Description}), "application/xlsx", "results.xlsx");
                case "csv":
                    return File(System.Text.Encoding.UTF8.GetBytes(ICSV.CreateCSV(diseaseCourses.Cast<ICSV>().ToList())), "application/csv", "results.csv");
                case null:
                    break;
                default:
                    return BadRequest(new { errors = new { Message = new string[] { "Niepoprawna opcja typu pliku" } } });
            }

            if (diseaseCourses.Count() == 0)
            {
                return BadRequest(new { errors = new { Message = new string[] { "Nie znaleziono danych dla wybranych filtrów!" } } });
            }

            var result = diseaseCourses
                .GroupBy(x => x.DiseaseCourse.Name)
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / diseaseCourses.Count) * 100 });

            return Ok(result);
        }

        [HttpGet("treatments/{provinceId}/{dateFrom}/{dateTo}")]
        [HttpGet("treatments/{provinceId}/{dateFrom}/{dateTo}/{type}")]
        public async Task<IActionResult> GetTreatment(Guid provinceId, DateTime dateFrom, DateTime dateTo, string? type)
        {
            var treatments = await unitOfWork.TreatmentRepository.GetTreatments(provinceId, dateFrom, dateTo);

            switch (type)
            {
                case "xlsx":
                    return File(ExcelExtension.CreateExcel(treatments, new[] { "StartDate", "EndDate", "IsCovid", "TreatmentStatus" },
                        x => new object[] { x.StartDate.ToString(), x.EndDate?.ToString(), x.IsCovid, x.TreatmentStatus.Name }), "application/xlsx", "results.xlsx");
                case "csv":
                    return File(System.Text.Encoding.UTF8.GetBytes(ICSV.CreateCSV(treatments.Cast<ICSV>().ToList())), "application/csv", "results.csv");
                case null:
                    break;
                default:
                    return BadRequest(new { message = "Niepoprawna opcja typu pliku" });
            }

            if (treatments.Count() == 0)
            {
                return BadRequest(new { errors = new { Message = new string[] { "Nie znaleziono danych dla wybranych filtrów!" } } });
            }

            var result = treatments
                .GroupBy(x => x.TreatmentStatus.Name)
                .Select(x => new { Key = x.Key, Proc = ((double)x.Count() / treatments.Count) * 100 });

            return Ok(result);
        }
    }
}
