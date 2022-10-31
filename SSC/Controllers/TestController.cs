using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.UnitOfWork;
using SSC.DTO.Place;
using SSC.DTO.Test;

namespace SSC.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : CommonController
    {
        private readonly IUnitOfWork unitOfWork;

        public TestController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("addTest")]
        public async Task<IActionResult> AddTest(TestCreateDTO test)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.TestRepository.AddTest(test, issuerId);

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

        [HttpPut("editTest")]
        public async Task<IActionResult> EditTest(TestUpdateDTO test)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await unitOfWork.TestRepository.EditTest(test, issuerId);
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
                var result = await unitOfWork.TestRepository.ShowTests(patientId);
                if(!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(unitOfWork.Mapper.Map<List<TestOverallGetDTO>>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpGet("testDetails/{testId}")]
        public async Task<IActionResult> TestDetails(Guid testId)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.TestRepository.TestDetails(testId);
                if (!result.Success)
                {
                    return BadRequest(new { errors = new { Message = new string[] { result.Message } } });
                }
                return Ok(unitOfWork.Mapper.Map<TestGetDTO>(result.Data));
            }
            return BadRequest(new { errors = new { Message = new string[] { "Invalid data" } } });
        }

        [HttpPost("addPlace")]
        public async Task<IActionResult> AddPlace(PlaceCreateDTO place)
        {
            if (ModelState.IsValid)
            {
                var result = await unitOfWork.PlaceRepository.AddPlace(place);

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
    }
}
