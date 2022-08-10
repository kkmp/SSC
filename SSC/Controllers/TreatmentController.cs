using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSC.Data.Repositories;
using SSC.Models;
using System.Web.Http.Cors;

namespace SSC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class TreatmentController : CommonController
    {
        private IConfiguration _config;
        private readonly ITreatmentRepository treatmentRepository;
        private readonly IMapper mapper;

        public TreatmentController(IConfiguration config, ITreatmentRepository treatmentRepository, IMapper mapper)
        {
            _config = config;
            this.treatmentRepository = treatmentRepository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpPost("addTreatment")]
        public async Task<IActionResult> AddTreatment(TreatmentViewModel treatment)
        {
            if (ModelState.IsValid)
            {
                var id = GetUserId();
                var result = await treatmentRepository.AddTreatment(treatment, id);
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
        public async Task<IActionResult> ShowTreatments(IdViewModel patientId)
        {
            if (ModelState.IsValid)
            {
                var result = await treatmentRepository.ShowTreatments(patientId.Id);
                //select się może popsuć gdy null
                return Ok(result.Select(x => new
                {
                    StartDate = x.StartDate?.ToString(),
                    EndDate = x.EndDate?.ToString(),
                    x.IsCovid,
                    UserName = x.User.Name,
                    UserSurname = x.User.Surname,
                    UserRole = x.User.Role.Name,
                    TreatmentStatus = x.TreatmentStatus.Name
                }));
            }
            return BadRequest(new { message = "Invalid data" });
        }
    }
}
