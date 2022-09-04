using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.DTO;
using SSC.Models;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentController : CommonController
    {
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IMapper mapper;

        public TreatmentController(ITreatmentRepository treatmentRepository, IMapper mapper)
        {
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addTreatment")]
        public async Task<IActionResult> AddTreatment(TreatmentCreateDTO treatment)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await treatmentRepository.AddTreatment(treatment, issuerId);
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

        [HttpGet("showTreatments")]
        public async Task<IActionResult> ShowTreatments(IdCreateDTO patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentRepository.ShowTreatments(patientId.Id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(mapper.Map<List<TreatmentGetDTO>>(result.Data));
            }
            return BadRequest(new { message = "Invalid data" });
        }

        [HttpPut("editTreatment")]
        public async Task<IActionResult> EditTreatment(TreatmentUpdateDTO treatment)
        {
            if (ModelState.IsValid)
            {
                var issuerId = GetUserId();
                var result = await treatmentRepository.EditTreatment(treatment, issuerId);
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
