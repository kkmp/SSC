using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.DTO;
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

        public TestController(IConfiguration config, ITestRepository testRepository, IMapper mapper)
        {
            _config = config;
            this.testRepository = testRepository;
            this.mapper = mapper;
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
                //poprawić
                var result = await testRepository.ShowTests(patientId.Id);
                /*if(!result.Success)
                {
                    return BadRequest(result.Message);
                }
                */
                return Ok(mapper.Map<List<TestOverallDTO>>(result));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpGet("testDetails")]
        public async Task<IActionResult> TestDetails(IdViewModel testId)
        {
            if (ModelState.IsValid)
            {
                //null?
                var result = await testRepository.TestDetails(testId.Id);
                return Ok(mapper.Map<TestDTO>(result));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
