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
        public async Task<IActionResult> AddTest(TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await testRepository.AddTest(test, issuerId);
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
                var issuerId = GetUserId();
                var result = await testRepository.EditTest(test, issuerId);
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
                if(!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(mapper.Map<List<TestOverallDTO>>(result.Data));
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
