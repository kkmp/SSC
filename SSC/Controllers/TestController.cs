using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.Models;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class TestController : CommonController
    {
        private IConfiguration _config;
        private readonly ITestRepository testRepository;
        private readonly IMapper mapper;

        public TestController(IConfiguration config, ITestRepository testRepository, IRoleRepository roleRepository)
        {
            _config = config;
            this.testRepository = testRepository;
        }

        [HttpPost("addTest")]
        public async Task<IActionResult> AddTest([FromBody] TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await testRepository.AddTest(test, id);
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

        [HttpPut("editTest")]
        public async Task<IActionResult> EditTest(TestEditViewModel test)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await testRepository.EditTest(test, id);
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

        [HttpGet("showTests")]
        public async Task<IActionResult> ShowTests(IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await testRepository.ShowTests(patientId.Id);
                //select się może popsuć gdy null
                return Ok(result.Select(x => new
                {
                    TestDate = x.TestDate?.ToString(),
                    x.OrderNumber,
                    ResultDate = x.ResultDate?.ToString(),
                    x.Result,
                    TestType = x.TestType.Name,
                    Place = x.Place.Name
                }));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpGet("testDetails")]
        public async Task<IActionResult> TestDetails(IdViewModel testId)
        {
            if (ModelState.IsValid)
            {
                var result = await testRepository.TestDetails(testId.Id);
                return Ok(new //obsługa dla null
                {
                    TestDate = result.TestDate?.ToString(),
                    result.OrderNumber,
                    ResultDate = result.ResultDate?.ToString(),
                    result.Result,
                    TestType = result.TestType.Name,
                    result.TreatmentId, //być może oddzielne zapytanie o te dane np. po kliknięciu po więcej info
                    UserName = result.User.Name,
                    UserSurname = result.User.Surname,
                    UserRole = result.User.Role.Name,
                    Place = result.Place.Name
                });
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
