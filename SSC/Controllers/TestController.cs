using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Models;
using SSC.Data.Repositories;
using SSC.Models;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class TestController : CommonController
    {
        private IConfiguration _config;
        private readonly ITestRepository testRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;

        public TestController(IConfiguration config, ITestRepository testRepository, IRoleRepository roleRepository)
        {
            _config = config;
            this.testRepository = testRepository;
            this.roleRepository = roleRepository;
        }

        [Authorize]
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
    }
}
