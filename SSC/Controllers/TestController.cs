using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO.Test;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : CommonController
    {
        private readonly ITestRepository testRepository;
        private readonly IMapper mapper;

        public TestController(ITestRepository testRepository, IMapper mapper)
        {
            this.testRepository = testRepository;
            this.mapper = mapper;
        }

        [HttpPost("addTest")]
        public async Task<IActionResult> AddTest(TestCreateDTO test)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await testRepository.AddTest(test, issuerId);
                var msg = new { errors = new { Message = new string[] { result.Message } } }; //!!!!!!!!!!!!
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
        public async Task<IActionResult> EditTest(TestUpdateDTO test)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await testRepository.EditTest(test, issuerId);
                if (result.Success)
                {
                    return Ok(new { message = result.Message });
                }
                else
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });

                }
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("showTests/{patientId}")]
        public async Task<IActionResult> ShowTests(Guid patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await testRepository.ShowTests(patientId);
                if(!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(mapper.Map<List<TestOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("testDetails/{testId}")]
        public async Task<IActionResult> TestDetails(Guid testId)
        {
            if (ModelState.IsValid)
            {
                var result = await testRepository.TestDetails(testId);
                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(mapper.Map<TestGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }
    }
}
